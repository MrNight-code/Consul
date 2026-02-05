
import re
import os

base_dir = os.path.dirname(os.path.abspath(__file__))
parent_dir = os.path.dirname(base_dir)

input_path = os.path.join(parent_dir, "syscons1_bdbosquescolina.sql")
create_path = os.path.join(parent_dir, "Divided", "01_create_tables.sql")
insert_path = os.path.join(parent_dir, "Divided", "02_insert_data.sql")
alter_path = os.path.join(parent_dir, "Divided", "03_alter_tables.sql")

print("Starting intelligent split and merge...")

# Regex patterns
re_create_header = re.compile(r"CREATE TABLE\s+(?:IF NOT EXISTS\s+)?`?(\w+)`?", re.IGNORECASE)
re_alter_pk = re.compile(r"ALTER TABLE\s+`?(\w+)`?\s+ADD\s+PRIMARY\s+KEY\s*\(([^)]+)\)", re.IGNORECASE)
re_alter_ai = re.compile(r"ALTER TABLE\s+`?(\w+)`?\s+MODIFY\s+`?(\w+)`?.*?AUTO_INCREMENT", re.IGNORECASE)
re_insert = re.compile(r"^INSERT INTO", re.IGNORECASE)

statements = []
try:
    # 1. Read entire file and separate into distinct statements
    with open(input_path, 'r', encoding='utf-8', errors='replace') as infile:
        buffer = ""
        for line in infile:
            stripped = line.strip()
            if not stripped or stripped.startswith("--") or stripped.startswith("/*"):
                continue
            
            buffer += line
            if stripped.endswith(";"):
                statements.append(buffer.strip())
                buffer = ""

    print(f"Parsed {len(statements)} SQL statements.")

    # 2. Categorize and Index
    create_stmts = {} # table_name -> full_statement_string
    pk_alters = {}    # table_name -> [column_names_str]
    ai_alters = {}    # table_name -> column_name
    other_alters = []
    inserts = []
    others = []

    for stmt in statements:
        upper_stmt = stmt.upper()
        # Simplify statement for regex matching (remove newlines usually helps if regex isn't multiline mode)
        clean_one_line = " ".join(stmt.split())

        if upper_stmt.startswith("CREATE TABLE"):
            m = re_create_header.search(stmt)
            if m:
                table_name = m.group(1)
                create_stmts[table_name] = stmt
            else:
                others.append(stmt) # Should not happen often
        
        elif upper_stmt.startswith("INSERT INTO"):
            stmt_ignore = re_insert.sub("INSERT IGNORE INTO", stmt)
            inserts.append(stmt_ignore)
            
        elif upper_stmt.startswith("ALTER TABLE"):
            # Check for AI
            m_ai = re_alter_ai.search(clean_one_line)
            # Check for PK
            m_pk = re_alter_pk.search(clean_one_line)
            
            if m_ai:
                table = m_ai.group(1)
                col = m_ai.group(2)
                ai_alters[table] = col
            elif m_pk:
                table = m_pk.group(1)
                cols = m_pk.group(2) # "pk_id", "id2"
                pk_alters[table] = cols
            else:
                other_alters.append(stmt)
        elif upper_stmt.startswith("SET ") or upper_stmt.startswith("START ") or upper_stmt.startswith("COMMIT"):
            continue 
        elif upper_stmt.startswith("DROP TABLE"):
            continue
        else:
            others.append(stmt)

    print(f"Classified: {len(create_stmts)} Creates, {len(inserts)} Inserts, {len(pk_alters)} PK-Alters, {len(ai_alters)} AI-Alters.")

    seen_tables_lower = {} # low_name -> original_name_first_seen
    renames = {} # original_name -> new_unique_name

    # 3. Merge Phase
    final_creates = []
    
    # We iterate through the captured CREATE statements
    # Note: create_stmts is a dict, order is preservation of insertion in Python 3.7+
    for table, stmt in create_stmts.items():
        # Handle Collision for Case-Insensitive FS (Windows)
        t_low = table.lower()
        if t_low in seen_tables_lower:
            # Collision detected
            prev_name = seen_tables_lower[t_low]
            if prev_name != table:
                # Different casing, same letters -> Rename this new one
                new_name = f"{table}_dup"
                renames[table] = new_name
                print(f"Renaming collision: {table} -> {new_name} (Clash with {prev_name})")
                table_to_write = new_name
                
                # We need to calculate the new stmt with the renamed table
                # Replace the table name in the first line
                stmt = re_create_header.sub(f"CREATE TABLE `{new_name}`", stmt, count=1)
            else:
                table_to_write = table
        else:
            seen_tables_lower[t_low] = table
            table_to_write = table

        # Get modifications for this table (using ORIGINAL name for lookup in alters)
        pk_cols = pk_alters.get(table)
        ai_col = ai_alters.get(table)
        
        lines = stmt.splitlines()
        new_lines = []
        
        # ... logic for processing lines ...
        
        pk_injected = False
        
        for i, line in enumerate(lines):
            # Check if this line defines the AI column
            if ai_col and f"`{ai_col}`" in line:
                # Replace the definition.
                if "AUTO_INCREMENT" not in line.upper():
                    if line.strip().endswith(","):
                        line = line.replace(",", " AUTO_INCREMENT,", 1)
                    else:
                        line = line + " AUTO_INCREMENT"
            
            new_lines.append(line)
        
        if pk_cols:
            joined = "\n".join(new_lines)
            pk_def = f"  PRIMARY KEY ({pk_cols})"
            # Note: This regex finds the LAST `\n)` before table options
            joined = re.sub(r"\n\)(\s*ENGINE=|\s*DEFAULT|\s*;)", f",\n{pk_def}\n)\\1", joined, count=1, flags=re.IGNORECASE)
            
            final_creates.append(f"DROP TABLE IF EXISTS `{table_to_write}`;\n" + joined)
        else:
            final_creates.append(f"DROP TABLE IF EXISTS `{table_to_write}`;\n" + "\n".join(new_lines))

    # Process Inserts to apply renames
    final_inserts = []
    re_insert_table_capture = re.compile(r"INSERT IGNORE INTO\s+`?(\w+)`?", re.IGNORECASE)
    
    for ins in inserts:
        m = re_insert_table_capture.match(ins)
        if m:
            t_name = m.group(1)
            if t_name in renames:
                # Replace table name
                new_t = renames[t_name]
                ins = ins.replace(f"INTO `{t_name}`", f"INTO `{new_t}`", 1).replace(f"INTO {t_name} ", f"INTO `{new_t}` ", 1)
        final_inserts.append(ins)

    # Process Alters to apply renames
    final_alters = []
    # Simple regex to capture alter table name
    re_alter_table_capture = re.compile(r"ALTER TABLE\s+`?(\w+)`?", re.IGNORECASE)
    for alt in other_alters:
        m = re_alter_table_capture.match(alt)
        if m:
            t_name = m.group(1)
            if t_name in renames:
                new_t = renames[t_name]
                alt = alt.replace(f"TABLE `{t_name}`", f"TABLE `{new_t}`", 1).replace(f"TABLE {t_name} ", f"TABLE `{new_t}` ", 1)
        final_alters.append(alt)
        
    # 4. Write Outputs
    with open(create_path, 'w', encoding='utf-8') as f:
        f.write("SET FOREIGN_KEY_CHECKS=0;\n")
        f.write("SET SQL_MODE = \"NO_AUTO_VALUE_ON_ZERO\";\n")
        f.write("START TRANSACTION;\n")
        f.write("SET time_zone = \"+00:00\";\n\n")
        for c in final_creates:
            f.write(c + "\n\n")
        # Also write unknown/other creates if any
        # Assuming others don't have this collision or we can't easily parse them
        for o in others:
            f.write(o + "\n\n")
        f.write("SET FOREIGN_KEY_CHECKS=1;\n")
        f.write("COMMIT;\n")

    with open(insert_path, 'w', encoding='utf-8') as f:
        f.write("SET FOREIGN_KEY_CHECKS=0;\n")
        f.write("SET SQL_MODE = \"NO_AUTO_VALUE_ON_ZERO\";\n")
        f.write("START TRANSACTION;\n")
        f.write("SET time_zone = \"+00:00\";\n\n")
        for i in final_inserts:
            f.write(i + "\n\n")
        f.write("SET FOREIGN_KEY_CHECKS=1;\n")
        f.write("COMMIT;\n")

    with open(alter_path, 'w', encoding='utf-8') as f:
        f.write("SET FOREIGN_KEY_CHECKS=0;\n\n")
        for a in final_alters:
            f.write(a + "\n\n")
        f.write("SET FOREIGN_KEY_CHECKS=1;\n")

    print("Success. Files rewritten.")

except Exception as e:
    print(f"Error: {e}")
