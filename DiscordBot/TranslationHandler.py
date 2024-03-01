import pygoogletranslation

def Translate(tar, txt):
    TR = pygoogletranslation.Translator()
    result = TR.translate(txt.lower(), dest=tar).text
    return result