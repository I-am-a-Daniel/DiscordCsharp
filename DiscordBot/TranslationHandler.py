import pygoogletranslation
import sys
sys.stdout.reconfigure(encoding='utf-8')
sys.stdin.reconfigure(encoding='utf-8')
TR = pygoogletranslation.Translator()
entry = input("")
tar = entry.split("|")[0].strip()
txt = entry.split("|")[1]
result = "Not yet implemented"
if tar.find("en") != -1:
	result = TR.translate(text = txt.lower(), dest = "en").text
elif tar.find("hu") != -1:
	result = TR.translate(text = txt.lower(), dest = "hu").text
elif tar.find("de") != -1:
	result = TR.translate(text = txt.lower(), dest = "de").text
print(result)
