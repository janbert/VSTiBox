/**************************************************************************
*   $Id: atoi.c 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#include	<ctype.h>
#include	<stdlib.h>

int atoi(register const char * s)
{
	register char	c;
	register int	a;
	register unsigned char	sign;

skipws:
	c = *s;
	if(c == ' ' || c == '\t') {
		s++;
		goto skipws;
	}
	a = 0;
	sign = 0;
	if(c == '-') {
		sign++;
		s++;
	} else if(c == '+')
		s++;
conv:
	c = *s;
	if(isdigit(c)) {
		a = a*10 + (c - '0');
		s++;
		goto conv;
	}
	if(sign)
		return -a;
	return a;
}
