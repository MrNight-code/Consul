
path = r"e:\Pasantias\SBTC\Sistema Gabriel\db_antiguo\02_insert_data.sql"
with open(path, 'r', encoding='utf-8', errors='replace') as f:
    for line in f:
        if "TbReporteFinal" in line and "INSERT" in line:
            print("Found Insert:")
            print(line[:500]) # Print first 500 chars 
            break
