import hid
import configparser
import os

def is_mouse_device(device):
    return device['usage_page'] == 1 and device['usage'] == 2

def replace_guid_in_ini(ini_path):
    if not os.path.exists(ini_path):
        print(f"Le fichier INI '{ini_path}' n'existe pas.")
        return

    config = configparser.ConfigParser()
    config.read(ini_path)

    for section in config.sections():
        for key in config[section]:
            hidpath_line = config[section][key]
            last_hash_index = hidpath_line.rfind('#')
            if last_hash_index != -1:
                new_hidpath_line = hidpath_line[:last_hash_index + 1] + '{378de44c-56ef-11d1-bc8c-00a0c91405dd}'
                config[section][key] = new_hidpath_line

    with open(ini_path, 'w') as configfile:
        config.write(configfile, space_around_delimiters=True)

def get_hid_devices():
    ini_path = 'HIDsget.ini'
    config = configparser.ConfigParser()

    if os.path.exists(ini_path):
        os.remove(ini_path)

    all_devices = list(hid.enumerate())
    desired_product_ids = [0x8042, 0x8043, 0x8044, 0x8045, 0x1802]

    gun4ir_devices = []
    mayflash_devices = []

    for device in all_devices:
        product_id = device['product_id']
        if product_id in desired_product_ids and is_mouse_device(device):
            hidpath = bytes.decode(device['path']).replace('\\\\', '\\')
            hidpath = '\\' + hidpath
            if product_id in [0x8042, 0x8043, 0x8044, 0x8045]:
                gun4ir_devices.append(hidpath)
            elif product_id == 0x1802:
                mayflash_devices.append(hidpath)

    device_count = 1
    for hidpath in gun4ir_devices:
        config[f'GUN4IR_{device_count}'] = {f'gun4ir_{device_count}': hidpath}
        device_count += 1

    for hidpath in mayflash_devices:
        config[f'Mayflash_Wiimote_Adapter_{device_count}'] = {f'mayflash_wiimote_adapter_{device_count}': hidpath}
        device_count += 1

    with open(ini_path, 'w') as configfile:
        config.write(configfile, space_around_delimiters=True)

    replace_guid_in_ini(ini_path)

get_hid_devices()
