def common_values(lit1,lit2):
    for i in range(len(lit1)):
        if i in lit2:
            return True
        
    return False


assert common_values({1,2,3}), {3,4,5} == True
assert common_values({1,2,3}, {30,40,50}) == False
print("all tests pass!")