
# This Service converts currency (dollars) from numbers into word presentation in American English only.

The algorithm of converting the number to words like:
1) validate the input
2) split the dollars and cents into two integers
3) for numbers 0 to 19, we get the direct match. ie: 0,1,2,3,4,5,6,7,8,9, 11,12,13,14,15,16,17,18,19
4) for multiples of ten, we get the direct match. ie: 10,20,30,40,50,60,70,80,90
5) for the other numbers consist of two digits except multiples of ten, we combine the multiples of ten and the single digit. ie: 21,22,23,........97,89,99
6) for the other numbers, ie: 100 to 999,999,999  we split the number into millions, thousands, hundreds, rest and add the proper word to the end:
	ex: if the given number was 123,456,789. Then
	Millions = 123
	Thousands = 456
	Hundreds = 7
	Rest = 89
	
	with the proper word:
	Millions = 123 million
	Thousands = 456 thousand
	Hundreds = 7 handres
	Rest = 89
7) then we recurse the algorithm for each part using the "Recursion" concept of OOP.
8) finally we combine the parts together


