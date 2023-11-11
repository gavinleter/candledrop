def maximum(*values):
    max = None
    min = None
    for v in values:
        if max == None or v > max:
            max == v
        if min == None or v < min:
            min = v
    return min, max