'''
script: guessTheNumber
author: Gavin Lester
date: 2/8/2024
action: Lets user guess a random number 1 - 1000. Tells them if the number is too high or too low. Lets them keep guessing
        until they guess the correct number. Then, it returns a statement on how the player performed.
'''

#import random module
from random import randint

#define sentinel
playAgain = 'y'

#establish sentinel loop
while playAgain.lower() == 'y':

    #get random int between 1 and 1000 and establish guesses at zero
    answer = randint(1,1000)
    guessNumber = 0

    #establish each guess
    guess = int(input("Guess my number between 1 and 1000 in the fewest guesses."))


    while guess != answer:

        #response for if guess is greater than answer
        if guess > answer:
            guessPrompt = "Too high, try again"
            guessNumber += 1

        #response for if guess is less than answer
        elif guess < answer:
            guessPrompt = "Too low, try again"
            guessNumber += 1

        #prompt another guess
        guess = int(input(guessPrompt))

    #response for if guess is equal to the answer
   
    print("Congratulations. You guessed the number!")
    guessNumber += 1
        
    #count guesses and respond accordingly
    if guessNumber > 10:
        print("You should be able to do better!")
        
    else:
        print("Either you know the secret or you got lucky!")

    #ask if player wants to play again
    playAgain = str(input("Would you like to guess again? (y/n)"))