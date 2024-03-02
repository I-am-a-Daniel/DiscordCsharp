# -*- coding: utf-8 -*-
import pygoogletranslation
import sys
sys.stdout.reconfigure(encoding='utf-8')
sys.stdin.reconfigure(encoding='utf-8')
TR = pygoogletranslation.Translator()
tar = input("")
txt = input("")
result = TR.translate(txt.lower(), dest="en").text
print(result)
