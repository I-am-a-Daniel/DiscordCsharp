# -*- coding: utf-8 -*-
import pygoogletranslation
TR = pygoogletranslation.Translator()
tar = input("")
txt = input("")
result = TR.translate(txt.lower(), dest=tar).text
print(result)
