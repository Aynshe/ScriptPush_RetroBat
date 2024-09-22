import hid
import configparser
import os

def is_mouse_device(device):
    return device['usage_page'] == 1 and device['usage'] == 2

def replace_guid_in_ini(ini_path):
    # Vérifier si le fichier INI existe
    if not os.path.exists(ini_path):
        print(f"Le fichier INI '{ini_path}' n'existe pas.")
        return

    # Lire le fichier INI
    config = configparser.ConfigParser()
    config.read(ini_path)

    # Remplacer les GUID dans les lignes HIDPath
    for section_name in config.sections():
        if section_name in config[section_name]:
            hidpath_line = config[section_name][section_name]
            # Trouver la dernière occurrence de "#" dans la ligne HIDPath
            last_hash_index = hidpath_line.rfind('#')
            if last_hash_index != -1:
                # Remplacer le GUID par {378de44c-56ef-11d1-bc8c-00a0c91405dd} après le dernier "#"
                new_hidpath_line = hidpath_line[:last_hash_index + 1] + '{378de44c-56ef-11d1-bc8c-00a0c91405dd}'
                config[section_name][section_name] = new_hidpath_line

    # Écrire dans le fichier INI en écrasant les données existantes
    with open(ini_path, 'w') as configfile:
        config.write(configfile, space_around_delimiters=True)  # Ajout d'espaces autour des délimiteurs

def get_hid_devices():
    # Chemin du fichier INI
    ini_path = 'HIDsget.ini'

    # Créer un objet ConfigParser
    config = configparser.ConfigParser()

    # Supprimer toutes les données existantes dans le fichier INI
    if os.path.exists(ini_path):
        os.remove(ini_path)

    # Trouver tous les périphériques HID
    all_devices = list(hid.enumerate())

    # Définir les ProductIDs souhaités
    desired_product_ids = [0x8042, 0x8043, 0x8044, 0x8045, 0x1802]

    # Définir un dictionnaire pour suivre le nombre de périphériques GUN4IR et Mayflash trouvés
    gun4ir_count = 0
    mayflash_count = {}

    # Parcourir et enregistrer les détails des périphériques HID
    for index, device in enumerate(all_devices):
        product_id = device['product_id']
        if product_id in desired_product_ids and is_mouse_device(device):
            if product_id == 0x8042:
                gun4ir_count += 1
                section_name = f'GUN4IR_P{gun4ir_count}'
            elif product_id == 0x8043:
                gun4ir_count += 1
                section_name = f'GUN4IR_P{gun4ir_count}'
            elif product_id == 0x8044:
                gun4ir_count += 1
                section_name = f'GUN4IR_P{gun4ir_count}'
            elif product_id == 0x8045:
                gun4ir_count += 1
                section_name = f'GUN4IR_P{gun4ir_count}'
            elif product_id == 0x1802:
                mayflash_count.setdefault(gun4ir_count, 0)
                mayflash_count[gun4ir_count] += 1
                section_name = f'Mayflash_Wiimote_Adapter_{mayflash_count[gun4ir_count]}'

            # Convertir le chemin de bytes à str avant de le manipuler
            hidpath = bytes.decode(device['path']).replace('\\\\', '\\')
            hidpath = '\\' + hidpath  # Ajouter la barre oblique manquante

            config[section_name] = {
            #    'VendorID': hex(device['vendor_id']),
            #   'ProductID': hex(product_id),
            #   'SerialNumber': device['serial_number'] or 'None',
                section_name: hidpath  # Remplacer 'HIDPath' par le nom de la section
            }

    # Écrire dans le fichier INI
    with open(ini_path, 'w') as configfile:
        config.write(configfile, space_around_delimiters=True)  # Ajout d'espaces autour des délimiteurs

    # Appeler la fonction pour remplacer les GUID dans le fichier INI
    replace_guid_in_ini(ini_path)

# Appeler la fonction
get_hid_devices()
