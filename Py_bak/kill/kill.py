import pygame
import sys
import os
import configparser
import logging
import subprocess  # Importation pour lancer des commandes externes
import xml.etree.ElementTree as ET

# Configuration du logging
logging.basicConfig(level=logging.INFO) 

# Chargement de la configuration
config = configparser.ConfigParser()

import os

def load_input_config():
    # Obtention du chemin du répertoire courant
    current_dir = os.getcwd()

    # Remonter de cinq niveaux dans l'arborescence des dossiers 
    for _ in range(3):
        current_dir = os.path.dirname(current_dir)

    # Construction du chemin vers es_input.cfg
    path_to_es_input = os.path.join(current_dir, 'emulationstation', '.emulationstation', 'es_input.cfg')
   
    # Lecture du fichier de configuration des entrées
    tree = ET.parse(path_to_es_input)
    root = tree.getroot()

    # Dictionnaire pour stocker les identifiants des touches
    devices_key_ids = {}

    # Traitement de chaque configuration d'entrée
    for inputConfig in root.findall('inputConfig'):
        device_type = inputConfig.get('type')
        device_name = inputConfig.get('deviceName') 
        device_guid = inputConfig.get('deviceGUID')

        key_ids = {'start': [], 'pageup': [], 'hotkey': []}
        for inputEntry in inputConfig.findall('input'):
            name = inputEntry.get('name')
            id = inputEntry.get('id')
            if name in key_ids:
                key_ids[name].append(int(id))

        devices_key_ids[(device_type, device_name, device_guid)] = key_ids

    return devices_key_ids

def main_loop():
    devices_key_ids = load_input_config()

    pygame.init()
    pygame.joystick.init()
    joysticks = [pygame.joystick.Joystick(i) for i in range(pygame.joystick.get_count())]
    for joystick in joysticks:
        joystick.init()

    joystick_states = {joystick.get_instance_id(): {} for joystick in joysticks}

    clock = pygame.time.Clock()
    update_interval = 50  # en millisecondes
    
    script_running = False

    while True:
        pygame.event.pump()

        for joystick in joysticks:
            instance_id = joystick.get_instance_id()
            guid = cleanGuid(joystick.get_guid())
            joystick_keys = get_device_keys(guid, devices_key_ids)

            buttons = joystick.get_numbuttons()
            for button_index in range(buttons):
                button_pressed = joystick.get_button(button_index)
                joystick_states[instance_id][button_index] = button_pressed

                hotkey_id = joystick_keys.get('hotkey', [None])[0]
                start_id = joystick_keys.get('start', [None])[0]

                hotkey_pressed = joystick_states[instance_id].get(hotkey_id, False)
                start_pressed = joystick_states[instance_id].get(start_id, False)

            if hotkey_pressed and start_pressed and not script_running:
                script_running = True
                launch_kill_script()
                script_running = False

        clock.tick(update_interval)                      

    pygame.quit()
    sys.exit()

def launch_kill_script():
    
    current_dir = os.getcwd()
    for _ in range(0):
        current_dir = os.path.dirname(current_dir)
    
    bat_path = os.path.join(current_dir, '.taskkill', 'taskkill.bat')
    subprocess.run([bat_path], shell=True)


def cleanGuid(guid):
    return guid[:4] + '0000' + guid[8:]

def get_device_keys(guid, devices_key_ids):
    for (device_type, device_name, device_guid), keys in devices_key_ids.items():
        if device_type == "joystick" and device_guid == guid:
            return keys
    return {}

if __name__ == "__main__":
    main_loop()
