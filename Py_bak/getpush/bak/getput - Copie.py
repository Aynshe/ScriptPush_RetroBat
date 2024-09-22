import configparser
import re
import os

def condition_met(config_lines, section, condition):
    if not condition:
        return True

    section_header = f"[{section}]"
    section_found = False
    for line in config_lines:
        if line.strip().lower() == section_header.lower():
            section_found = True
        elif line.strip().startswith('[') and section_found:
            # Si une nouvelle section commence, arrête la recherche
            break
        elif section_found and condition in line:
            return True
    return False

def clear_target_values(file_path, section, fields):
    with open(file_path, 'r') as file:
        lines = file.readlines()

    section_found = False
    section_start = -1
    for i, line in enumerate(lines):
        if line.strip().lower() in [f"{section.lower()}", f";{section.lower()}"]:
            section_found = True
            section_start = i
        elif section_found and line.strip().startswith('['):
            # Si une nouvelle section commence, arrête la recherche
            break

    # if section_start != -1:
        # # Trouver et supprimer les lignes avec les champs spécifiés
        # for field in fields:
            # for j in range(section_start, len(lines)):
                # if lines[j].strip().lower().startswith(f"{field.lower()} ="):
                    # lines[j] = f"{field} = \n"  # Ne pas ajouter un point-virgule devant la ligne

    # with open(file_path, 'w') as file:
        # file.writelines(lines)

def update_target_config(file_path, section, key, new_value):
    with open(file_path, 'r') as file:
        lines = file.readlines()

    section_found = False
    key_found = False
    for i, line in enumerate(lines):
        if line.strip().lower() in [f"{section.lower()}", f";{section.lower()}"]:
            section_found = True
        if section_found and line.strip().startswith(f"{key} ="):
            lines[i] = f"{key} = {new_value}\n"
            key_found = True
            break

    if not key_found and section_found:
        lines.append(f"{key} = {new_value}\n")

    with open(file_path, 'w') as file:
        file.writelines(lines)

def process_instruction(line):
    print(f"Traitement de la ligne : {line}")
    pattern = r"file_source:'(.+?)' section_source:'(.+?)' field_source:'(.+?)'(?: if_source:'(.+?)')? file_target:'(.+?)' section_target:'(.+?)' field_target:'(.+?)'"
    match = re.match(pattern, line)

    if match:
        file_source, section_source, field_source, if_source, file_target, section_target, field_target = match.groups()
        print(f"Source : {file_source}, Section Source : {section_source}, Champ Source : {field_source}, Condition Source : {if_source}")
        print(f"Cible : {file_target}, Section Cible : {section_target}, Champ Cible : {field_target}")

        if file_target == 'config.ini':
            # Nettoyer les valeurs spécifiées avant l'injection
            clear_target_values(file_target, section_target, [field_target])

        with open(file_source, 'r') as f:
            source_lines = f.readlines()

        if condition_met(source_lines, section_source, if_source):
            value = None
            for line in source_lines:
                if line.strip().startswith(f"{field_source} ="):
                    value = line.split('=')[1].strip()
                    break

            if value:
                update_target_config(file_target, section_target, field_target, value)
                print(f"Valeur {value} écrite dans {section_target}[{field_target}] du fichier cible")
                return True
            else:
                print("Valeur non trouvée, aucune écriture dans le fichier cible.")
        else:
            print("La condition spécifiée n'est pas remplie. Aucune action effectuée.")
    else:
        print(f"Impossible d'analyser la ligne : {line}")

    return False

def execute_instructions_from_file(file_path):
    with open(file_path, 'r') as file:
        sections_added = set()
        values_found = False
        for line in file:
            match = re.match(r"file_source:.*section_source:'(.+?)'", line)
            if match:
                section_source = match.group(1)
                sections_added.add(section_source)

            values_found |= process_instruction(line.strip())

        return values_found, sections_added

def process_config_files():
    run_files = ['gun4ir.run', 'mayflash.run']
    gun4ir_sections = set()
    mayflash_sections = set()

    for run_file in run_files:
        if os.path.exists(run_file):
            values_found, sections_added = execute_instructions_from_file(run_file)

            if run_file == 'gun4ir.run':
                gun4ir_sections.update(sections_added)
            else:
                mayflash_sections.update(sections_added)

    # Ajouter les sections manquantes de Mayflash_Wiimote_Adapter_2 et Mayflash_Wiimote_Adapter_3
    sections_to_add = set(gun4ir_sections) - set(mayflash_sections)
    for section_to_add in sections_to_add:
        for i in range(2, 4):
            execute_instructions_from_file('mayflash.run')

    print("Traitement terminé.")

if __name__ == "__main__":
    process_config_files()
