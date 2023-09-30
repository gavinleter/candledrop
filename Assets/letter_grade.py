def letter_grade(float):
    if float >= 90 and float <= 100:
        return ("A")
    if float >= 80 and float < 90:
        return ("B")
    if float >= 70 and float < 80:
        return ("C")
    if float >= 60 and float < 70:
        return ("D")
    if float < 60 and float >= 0:
        return ("E")
    else:
        return ("X")
    
def pass_or_fail(letter_grade):
    if len(letter_grade) > 1:
        return "Error"
    if letter_grade in "ABCD":
        return "Pass"
    else:
        return "Fail"
    
def point_grade(score,total_points):
    percent_grade = (score / total_points) * 100
    rounded_grade = round(percent_grade,2)
    return rounded_grade

def get_grade_results(score,total_points):
    grade_point = point_grade(score,total_points)
    grade_letter = letter_grade(grade_point)
    did_pass = pass_or_fail(grade_letter)
    return str("Your grade is " + str(grade_point) + " (" + str(grade_letter) + " - " + str(did_pass) + ")")