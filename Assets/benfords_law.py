'''
Gavin Lester
CSC110
Project 5
This program checks if the input files follows benfords law.
'''
def isfloat(i):
    '''making isfloat function to see if i is a float.'''
    for x in i:
        if x not in '0123456789.':
            return False
    return True

def csv_to_list(file_name):
    '''
    This function returns a list of integers, floats, and splits each element via a comma.
    Returns a list with numbers from the strings.
    '''
    list = []
    f = open(file_name , 'r')
    for line in f:
        #defining the index in each line while iterating 
        # through each line in file_name
        for i in line.strip("\n").split(","):
            #removes the line gaps
            #  and splits elements by comma
            if i.isnumeric() or isfloat(i):
                list.append(i)
    return list

def count_start_digits(numbers):
    '''This function takes in a list of numbers
    and returns a dictionary with counts of the leading digits'''
    dictionary = {1:0 , 2:0 , 3:0 , 4:0 , 5:0 , 6:0 , 7:0 , 8:0 , 9:0}
    for i in numbers:
        if int(i[0]) > 0:
            #converts leading digit(s) into integer(s)
            dictionary[int(i[0])] += 1
            #returns new dictionary
    return dictionary

def digit_percentages(counts):
    '''This function creates new dictionary with percentages for the frequency of each leading digit
    and returns a new dictionary with percentages of the leading digits'''
    dict = {1:0 , 2:0 , 3:0 , 4:0 , 5:0 , 6:0 , 7:0 , 8:0 , 9:0}
    total = 0
    for i in counts:
        total += counts[i]
        #counts frequency total
    for j in counts:
        dict[j] = round((counts[j]/total) * 100 , 2)
        #calculates percentages
    return dict


def check_benfords_law(percentage):
     '''This function determines if the dictionary follows Benford's Law, and then
     returns True or False based on whether it does or not'''
     
     dict_1 = {1:30 , 2:17 , 3:12 , 4:9 , 5:7 , 6:6 , 7:5 , 8:5 , 9:4}
     #benfords law values above
     dict_2 = percentage
     for key in dict_1.keys():
         #code above makes sure only the keys are used
         x = dict_1[key]
         y = dict_2[key]
         if y > x+10 or y < x-5:
             #compares entered dictionary to benford's law dictionary
             return False
     return True

#True or False

#done done done!!!!!!!