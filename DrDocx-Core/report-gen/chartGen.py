import matplotlib.pyplot as plt; plt.rcdefaults()
import numpy as np
import matplotlib.pyplot as plt
import os
import json
import sys
from colorama import init, Fore
from PIL import Image
import matplotlib.image as mpimg
import random	
import math


init() #colorama


#Test results will contain tuples of the tests results rom the ingest engine
#_TestResultGroup = []

#Contain all of the organized groups
_TestGroups = []
_decodedJson = dict()
_localDir = ""

_jsonPath = ""
_writePath = ""

_count = 0.0
_totalDotsH = 0.0

DEBUG = False
SHOWCHART = False
BOGUSVALS = True
FIRSTPAGE = True
DONE = False
AUTOPATH = False
PAGECOUNT = 1

def main():
	global DONE 
	global _jsonPath
	global _writePath

	if len(sys.argv) < 4:
		print(sys.argv[1])
		_jsonPath = sys.argv[1]
		_writePath = sys.argv[2]

	print(Fore.GREEN + "Generating Table")
	ingestDataFile()
	generateChart()
	#saveChart()
	print(Fore.BLUE + "Generating Image Page	")
	#loadNormalCurve()
	concatImages()

#Assume the data file is in the same directory with name dataFile.txt
def ingestDataFile():
	print("Intaking Data File")
	global DEBUG
	global _decodedJson
	global _localDir
	global AUTOPATH
	global _jsonPath

	_localDir = os.getcwd()
	if DEBUG:
		print("Local Directory is: " + _localDir)

	if AUTOPATH: 
		_dataFilePath = _localDir + "\\data.json"
	else: 
		_dataFilePath = _jsonPath
		print(_dataFilePath)
	_dataFile = open(_dataFilePath, "r") #read only access of the data file

	_rawDataJson = _dataFile.read()
	_decodedJson = json.loads(_rawDataJson)
	#print(_decodedJson.pop("ATTENTION").pop("W4-DSFd"))

	_dataFile.close()

def generateChart():
	global DEBUG
	global _decodedJson
	global SHOWCHART
	global _count
	global BOGUSVALS
	global PAGECOUNT

	print("Generating Chart")

	_categories = []
	for _key in _decodedJson.keys():
		_categories.append(_key)

	if DEBUG:
		print("Categories: ")
		print(_categories)

	_xList = []
	_yList = []
	_colors = []

	for _cat in _categories:
		_xList.append(r"$\bf{" + str(_cat.replace(" ", "\ ")) + "}$") #magic, don't touch
		_yList.append(0)
		_colors.append('black')

		# _testList = []
		# for _key in _decodedJson[_cat].keys():
		# 	_testList.append(_key)
		
		for index in range(0, len(_decodedJson[_cat])):
			_testName = _decodedJson[_cat][index]['RelatedTest']['Name']
			_testVal = _decodedJson[_cat][index]['ZScore']
			if BOGUSVALS:
				_testVal = random.uniform(-5,5)
			_xList.append(_testName)
			_yList.append(_testVal)
			_colors.append(getColor(_testVal))


		_xList.append("")
		_yList.append(0)
		_colors.append('black')
	
	_xList.pop()
	_yList.pop()
	_colors.pop()

	while not len(_xList) % 36 == 0:
		_xList.append("")
		_yList.append(0)
		_colors.append('black')

	if DEBUG:
		print(_xList)
		print(_yList)

	#_xList.reverse()
	#_yList.reverse()
	#_colors.reverse()

	PAGECOUNT = math.ceil(len(_xList) / 36.0)
	for page in range(1, int(PAGECOUNT + 1)):
		_tmpX = _xList[0 + 36 * (page - 1): 36 * page]
		_tmpY = _yList[0 + 36 * (page - 1): 36 * page]
		_tmpcolors = _colors[0 + 36 * (page - 1): 36 * page]

		_tmpX.reverse()
		_tmpY.reverse()
		_tmpcolors.reverse()

		_count = len(_tmpX)
		_yScal = np.arange(_count)
		plt.barh(_yScal, _tmpY, color = _tmpcolors, align='center', alpha=1)
		plt.yticks(_yScal, _tmpX, rotation = 35, fontsize = 10)
		plt.xlim([-4, 4])
		plt.grid(b=True, axis = 'x')
		plt.subplots_adjust(left = 0.25)
		#plt.tight_layout(pad = 0.5)
	#	if SHOWCHART:
	#		plt.show()
		saveChart(page)

def getColor(_val):
	#color = 'green'

	if _val < 0 and _val >= -1:
		color = 'yellow'
	elif _val < -1 and _val >= -2:
		color = 'orange'
	elif _val < -2: 
		color = 'red'
	else: 
		color = 'green'

	return color


def saveChart(pageNumber):
	global _localDir
	global _writePath

	print(Fore.YELLOW + "Saving Graph " + str(pageNumber))
	fig = plt.gcf()
	fig.set_size_inches(8, 10.5)
	fig.savefig(_writePath + "\\graph" + str(pageNumber) + ".png", tranparent=True, dpi = 300, orientation = 'portrait', pad_inches = 0)
	plt.clf()

def loadNormalCurve():
	print("Loading Curve Image")
	img = mpimg.imread(_localDir + "\\NormalCurve.png")

def concatImages():
	global PAGECOUNT
	global _writePath

	for _page in range(1, int(PAGECOUNT + 1)):
		print(Fore.CYAN + "Generating Visualization Page " + str(_page))
		_curve = Image.open(_localDir + "\\NormalCurveResized.png")
		_table = Image.open(_writePath + "\\graph" + str(_page) + ".png")

		_dest = Image.new('RGB', (_table.width, _table.height + _curve.height), (255, 255, 255))
		_dest.paste(_table, (0, _curve.height - 470))
		_dest.paste(_curve, (186, 0))

		_dest = _dest.crop((0, 0, _dest.width, 300 * 10))
		_dest.save(_writePath + "\\renderedVisualization" + str(_page) + ".png")

	# print("PRINTING PAGE: " + str(PAGECOUNT))

	# _curve = Image.open(_localDir + "\\NormalCurveResized.png")
	# _table = Image.open(_localDir + "\\graph" + str(PAGECOUNT) + ".png")

	# _dest = Image.new('RGB', (_table.width, _table.height + _curve.height), (255, 255, 255))

	# _dest.paste(_table, (0, _curve.height - 150))
	# _dest.paste(_curve, (55, 0))

	# _dest = _dest.crop((0, 0, _dest.width, 300 * 10))
	# _dest.save(_localDir + "\\finishedVisualization" + str(PAGECOUNT) + ".png")

	# _cutoff = _table.height - (3000 - 601 - 150)
	# print(_cutoff)
	# if _cutoff <= 0: 
	# 	DONE = True
	# else:
	# 	_table = _table.crop((0, _cutoff, _table.width, _table.height - _cutoff))
	# 	_table.save(_localDir + "\\graph" + str(PAGECOUNT + 1) + ".png")

	# FIRSTPAGE = False
	# PAGECOUNT = PAGECOUNT + 1

if __name__ == "__main__":
    main()