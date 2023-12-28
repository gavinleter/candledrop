def bubble_sort(lit):
    i = 0
    smol = 0
    big = 0
    while i < len(lit):
        if lit[i] > lit[i+1]:
            smol = lit[i+1]
            big = lit[i]
            lit[i] = smol
            lit[i+1] = big
        i += 1




# this one works, though im not sure if the one above does :/
def bubble_sort(lit):
    end = len(lit) - 1
    for i in range(len(lit) - 1):
        for j in range(end):
            if lit[j] > lit[j+1]:
                lit[j], lit[j+1] = lit[j+1], lit[j]
        end -= 1
    
    return lit