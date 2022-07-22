import os
import os.path
from os import path

import requests
import urllib.parse
import urllib.request
from requests.structures import CaseInsensitiveDict

assetsUrl = "https://firebasestorage.googleapis.com/v0/b/condominium-assets.appspot.com/o/"
count = 0;
page = 0;

headers = CaseInsensitiveDict()
headers["Accept"] = "application/json"


resp = requests.get(assetsUrl, headers=headers)

if (resp.status_code == 200):
	currentPageToken = resp.json()['nextPageToken']
		
	while currentPageToken != None:
		page += 1
		print("Page " + str(page) + " with nextPageToken: " + currentPageToken)
		for value in resp.json()['items']:
			count += 1

			pathLength = len(value['name'].split("/"))
			pathSuffix = value['name'].split("/")[pathLength-1]

			# huge files
			if pathSuffix == "rp_claudia_rigged_002_dif_gloss.tga" or pathSuffix == "rp_eric_rigged_001_dif_gloss.tga":
				print("Skipped huge file: " + pathSuffix)
				continue;

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
				print("File downloaded: " + value['name'])
			except OSError as error:
				print("error: " + str(error))

		print(str(count) + " files found in the page " + str(page))
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

				# huge files
				if pathSuffix == "rp_claudia_rigged_002_dif_gloss.tga" or pathSuffix == "rp_eric_rigged_001_dif_gloss.tga":
					print("Skipped huge file: " + pathSuffix)
					continue;

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
					print("File downloaded: " + value['name'])
				except OSError as error:
					print("error: " + str(error))

			print(str(count) + " files found")
else:
	print("status code != 200: " + str(resp.json()))