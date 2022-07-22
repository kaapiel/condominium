import os
import os.path
from os import path

import requests
import urllib.parse
import urllib.request
from requests.structures import CaseInsensitiveDict

assetsUrl = "https://firebasestorage.googleapis.com/v0/b/condominium-buildings.appspot.com/o/"
count = 0;
page = 0;

headers = CaseInsensitiveDict()
headers["Accept"] = "application/json"


resp = requests.get(assetsUrl, headers=headers)

if (resp.status_code == 200):
	currentPageToken = ""
	try:
		currentPageToken = resp.json()['nextPageToken']
	except Exception as error:
		print("No more pages found")
		
	while currentPageToken != None:
		page += 1
		print("Page " + str(page) + " with nextPageToken: " + currentPageToken)
		for value in resp.json()['items']:
			count += 1

			pathLength = len(value['name'].split("/"))
			pathSuffix = value['name'].split("/")[pathLength-1]

			if path.exists(value['name']):
				print("File already exists. Skipping.")
				continue;

			try:
				os.makedirs(value['name'].removesuffix(pathSuffix))
			except Exception as e:
				print()
			
			encoded = assetsUrl + urllib.parse.quote(value['name'], safe='')
			subResp = requests.get(encoded, headers=headers)
			
			try:
				urllib.request.urlretrieve(encoded + "?alt=media&token=" + subResp.json()['downloadTokens'], value['name'])
			except OSError as error:
				print("error: " + str(error))

		print(str(count) + " files found in the page " + str(page))
		
		if (currentPageToken == ""):
			currentPageToken = None
			break;
		
		resp = requests.get(assetsUrl + '?pageToken=' + currentPageToken, headers=headers)
		
		try:
			currentPageToken = resp.json()['nextPageToken']
		except Exception as error:
			currentPageToken = None
			page += 1
			print("Page " + str(page) + " without nextPageToken")
			for value in resp.json()['items']:
				count += 1
				
				pathLength = len(value['name'].split("/"))
				pathSuffix = value['name'].split("/")[pathLength-1]

				if path.exists(value['name']):
					print("File already exists. Skipping.")
					continue;

				try:
					os.makedirs(value['name'].removesuffix(pathSuffix))
				except Exception as e:
					print()
			
				encoded = assetsUrl + urllib.parse.quote(value['name'], safe='')
				subResp = requests.get(encoded, headers=headers)
				
				try:
					urllib.request.urlretrieve(encoded + "?alt=media&token=" + subResp.json()['downloadTokens'], value['name'])
				except OSError as error:
					print("error: " + str(error))

			print(str(count) + " files found")
else:
	print("status code != 200")