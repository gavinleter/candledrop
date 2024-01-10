'''
Gavin Lester
CSC 110
Programming Project 6
Below is a text-based game 
'''
def load_game():
    """
    Loads game info from 'game.txt'.
    """
    game_file = open("game.txt", "r")
    game = {}

    for line in game_file:
        values = line.strip().split('\t')
        key = int(values[0])
        # Check if key is already in the dictionary, and append the new value
        if key in game:
            game[key].append(values[1:])
        else:
            game[key] = [values[1:]]

    game_file.close()
    return game

def load_objects():
    """
    Loads object info from the 'objects.txt'.
    """
    object_file = open("objects.txt", "r")
    objects = {}

    for line in object_file:
        values = line.strip().split('\t')
        key = (int(values[0]), int(values[1]), values[2])
        value = values[3:]
        objects[key] = value

    object_file.close()
    return objects

def load_travel_table():
    """
    Load travel table info from 'travel_table.txt'.
    """
    travel_file = open("travel_table.txt", "r")
    travel_table = {}

    for line in travel_file:
        values = line.strip().split('\t')
        key = (int(values[0]), int(values[1]))
        value = values[2]
        travel_table[key] = value

    travel_file.close()
    return travel_table

def print_instructions():
    """
    Print contents of 'instructions.txt'.
    """
    f = open("instructions.txt", "r")
    print(f.read())
    f.close()

def get_location(location, game, objects, player_objects):
    '''
    This function gives the location from the game data, and the next one too.
    there is no return statement
    Args:
        loc: integer
        game: dict with loc and string info
        objects: dict of loc, binary, and object name
        player_objects: list of strings

    '''
    # For each string associated with that location in the game
    # dictionary, print that line
    for line in game[location]:
        print(line)

    # Check if loc has object associated with it
    for key, value in objects.items():
        if key[0] == location and key[1] == 0 and key[2] not in player_objects:
            print(value[0])

        # If there's object associated with this location
        if key[0] == location and key[1] == 1:
            if key[2] in player_objects:
                # User has the object
                print(value[1])
            else:
                # User does not have the object
                print(value[0])

def go_to_location(location, travel_table, objects, player_objects, answer):
    '''
    This function checks for user's answer, 
    the objects that are available for the users to take, 
    and the objects the user has in their obj. list.
    It then returns the objects location.
    Args:
        location: integer
        travel_table: dictionary with current location, possible to go
                      location and verb that takes user to to go location
        objects: dictionary of location, binary (0 or 1), and object name
        player_objects: list of strings
        answer: input from the user (string)
    '''
    # Check if user wants to take an object
    if "take" in answer.lower():
        for key in objects:
            # Check if there's an object to take
            if key[0] == location and key[1] == 0:
                # Add object to user's object list
                player_objects.append(key[2])

    # Check if the user needs object to continue
    for key in objects:
        # If there's is object
        # but the user does not have it, return current location 
        if key[0] == location and key[1] == 1 and key[2] not in player_objects:
            return location

    # No objects to take or needed to go anywhere
    # Check where to go from user's answer
    for x_y, verb in travel_table.items():
        if x_y[0] == location and verb in answer.upper():
            return x_y[1]

def play_game():
    '''
    This function plays the whole game.
    It loads all files for the game, and asks
    for player's input

    (basically main)
    '''
    # Load game.txt
    game = load_game()
    # Load objects.txt
    objects = load_objects()
    # Load travel_table.txt
    travel_table = load_travel_table()
    # Player starts with no objects
    player_objects = []