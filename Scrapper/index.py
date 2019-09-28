import requests
import bs4
from bs4 import BeautifulSoup
import pandas as pd
import time
from datetime import datetime
from threading import Thread
import os
import glob
import json

max_results_per_city = 400
postionFind = [ "software+developer", "react+developer", 
                "devops", "software+engineer",
                "Machine+Learning+Engineer",
                "Data+Scientist"]

job_Type = "fulltime"
city_set = ["Ontario", "British+Columbia"]
max_age = "15"
host = "ca.indeed.com"
techTrans = "https://techtransparency93.azurewebsites.net/api/JobPostingsAPI/"
header = {"Content-type": "application/json",
          "Accept": "text/plain"} 
def dotheWork(city, pos, start, finalFileName):
    df_more = pd.DataFrame(
        columns=[
            "Title",
            "JobURL",
            "PostDate",
            "Location",
            "Company",
            "Salary",
            "Synopsis",
        ]
    )
    page = requests.get(
        "http://"
        + host
        + "/jobs?q="
        + pos
        + "+%2420%2C000&l="
        + str(city)
        + "&jt="
        + job_Type
        + "&fromage="
        + max_age
        + "&start="
        + str(start)
    )
    time.sleep(1)  # ensuring at least 1 second between page grabs
    soup = BeautifulSoup(page.text, "html.parser")
    for each in soup.find_all(class_="result"):
        try:
            title = each.find(class_="jobtitle").text.replace("\n", "").replace(",","")
        except:
            title = "NULL"
        #print(title)
        job_URL = "NULL"
        try:
            job_URL = "https://" + host + each.find(class_="jobtitle")["href"].replace(",","")
            
        except:
            job_URL = "NULL"
        try:
            if(job_URL is not 'NULL'):
                mainPage = requests.get(job_URL)
                time.sleep(1)
                DescriptionSoup = BeautifulSoup(mainPage.text, "html.parser")
                synopsis = str(DescriptionSoup.findAll("div", {"id": "jobDescriptionText"})[0].prettify().replace("\n", "").replace(",",""))
                #print(synopsis)
                if(synopsis is None):
                    continue
        except:
            synopsis ='NULL'

        try:
            location = each.find("span", {"class": "location"}).text.replace(
                "\n", ""
            ).replace(",","")
        except:
            location = "N/A"
        try:
            company = each.find(class_="company").text.replace("\n", "").replace(",","")
        except:
            company ="NULL"
        try:
            salary = each.find(class_="salary.no-wrap").text.replace(",","")
        except:
            salary ='N/A'

        try:
            PostDate = each.find(class_="date").text.replace("\n", "").replace(",","")
        except:
            PostDate = "N/A"
        
        body = {
                "title": title,
                "url": job_URL,
                "postDate": PostDate,
                "location": location,
                "company": company,
                "salary": salary,
                "summary": synopsis,
                "numberOfApplies": 0,
                "numberOfViews": 0,
                "poster": None,
                "posters":"ddd",
                "jobSource":"Indeed"
            }
        r = json.dumps(body)
        print(r)
        mainPage = requests.post(url =techTrans, data = r,headers=header)

    

def Indeed():
    workers = []
    now = datetime.now()
    dt_string = now.strftime("%d/%m/%Y %H:%M:%S").replace(" ", "").replace(":", "")
    finalFileName = ("JobPostingIndeed" + dt_string).replace("/", "")

    # scraping code:
    for city in city_set:
        for pos in postionFind:
            for start in range(0, max_results_per_city, 10):
                thread1 = Thread(
                    target=dotheWork,
                    args=(city, pos, start, city + pos + str(start) + finalFileName),
                )
                workers.append(thread1)

    for process in workers:
        process.start()

    for process in workers:
        process.join()


if __name__ == "__main__":
    start = time.time()

    print("Starting Download")
    Indeed()
    print("Completed Download")
   
    end = time.time()
    print(end - start)
