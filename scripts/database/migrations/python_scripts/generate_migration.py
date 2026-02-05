import os
import sys
import argparse

def generate_migration():
    parser = argparse.ArgumentParser(description="Generate a migration script from a template.")
    parser.add_argument("--source-db", help="Name of the source/legacy database (e.g., db_vieja_bosques)", required=False)
    parser.add_argument("--target-db", help="Name of the target database (e.g., consulcon)", required=False)
    parser.add_argument("--output-name", help="Name of the output file (e.g., migrated_bosques.sql)", required=False)
    parser.add_argument("--output-dir", help="Directory to save the output file", required=False)

    args = parser.parse_args()

    # Detect directories
    base_dir = os.path.dirname(os.path.abspath(__file__))
    template_path = os.path.join(base_dir, "migration_template.sql")
    migrations_root = os.path.dirname(base_dir) # Folder 'migrations/'

    print("--- Migration Script Generator ---")
    
    # 1. Get Database Names
    if args.source_db and args.target_db:
        source_db = args.source_db
        target_db = args.target_db
        filename = args.output_name if args.output_name else "migration_script.sql"
    else:
        # Interactive mode fallback
        source_db = input("1. Old Database Name (Source, e.g., db_vieja_bosques): ").strip()
        target_db = input("2. New Database Name (Target, e.g., consulcon): ").strip()
        filename_input = input("3. Output Filename [Enter for migration_script.sql]: ").strip()
        filename = filename_input if filename_input else "migration_script.sql"

    if not source_db or not target_db:
        print("Error: Both source and target databases must be specified.")
        return

    # 2. Read Template
    try:
        with open(template_path, "r", encoding="utf-8") as f:
            content = f.read()
    except FileNotFoundError:
        print(f"Error: Template not found at {template_path}")
        return

    # 3. Replace Variables
    new_content = content.replace("{{SOURCE_DB}}", source_db)
    new_content = new_content.replace("{{TARGET_DB}}", target_db)

    # 4. Determine output path
    if args.output_dir:
        output_dir = args.output_dir
    else:
        output_dir = migrations_root

    # Create directory if it doesn't exist
    if not os.path.exists(output_dir):
        try:
            os.makedirs(output_dir)
            print(f"Created directory: {output_dir}")
        except OSError as e:
            print(f"Error creating directory: {e}")
            return

    output_path = os.path.join(output_dir, filename)

    # 5. Save Generated File
    try:
        with open(output_path, "w", encoding="utf-8") as f:
            f.write(new_content)
        print(f"\nSuccess! Script generated at:\n{output_path}")
    except Exception as e:
        print(f"Error writing file: {e}")

if __name__ == "__main__":
    generate_migration()
