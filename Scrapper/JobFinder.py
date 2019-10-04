import requests
import bs4
from bs4 import BeautifulSoup
import time
from datetime import datetime
from threading import Thread
import json
import secret

max_results_per_city = 20
postionFind = [
    "software+developer",
    "react+developer",
    "devops",
    "software+engineer",
    "Machine+Learning+Engineer",
    "Data+Scientist",
    "junior+software+developer",
    "senior+software+developer",
]

job_Type = "fulltime"
city_set = ["Ontario", "British+Columbia"]
max_age = "10"
host = "ca.indeed.com"

techTrans = "https://techtransparency93.azurewebsites.net/api/JobPostingsAPI/"

techTransCheck = "https://techtransparency93.azurewebsites.net/api/JobPostingsAPI/Check"


header = {
    "Content-type": "application/json",
    "Accept": "text/plain",
    "auth": secret.getAuthKey(),
}


def checkifdup(urlIn):
    r = json.dumps({"url": urlIn})
    response = requests.post(url=techTransCheck, data=r, headers=header)
    time.sleep(1)
    print(response.content)
    result = json.loads(response.content)
    return result


def dotheWork(city, pos, start, finalFileName):
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
            title = each.find(class_="jobtitle").text.replace("\n", "").replace(",", "")
        except:
            title = "NULL"
        # print(title)
        job_URL = "NULL"
        try:
            job_URL = (
                "https://"
                + host
                + each.find(class_="jobtitle")["href"].replace(",", "")
            )

        except:
            job_URL = "NULL"
        try:
            if job_URL is not "NULL":
                mainPage = requests.get(job_URL)
                time.sleep(1)
                DescriptionSoup = BeautifulSoup(mainPage.text, "html.parser")
                synopsis = str(
                    DescriptionSoup.findAll("div", {"id": "jobDescriptionText"})[0]
                    .prettify()
                    .replace("\n", "")
                    .replace(",", "")
                )
                # print(synopsis)
                if synopsis is None:
                    continue
        except:
            synopsis = "NULL"

        try:
            location = (
                each.find("span", {"class": "location"})
                .text.replace("\n", "")
                .replace(",", "")
            )
        except:
            location = "N/A"
        try:
            company = (
                each.find(class_="company").text.replace("\n", "").replace(",", "")
            )
        except:
            company = "NULL"
        try:
            salary = each.find(class_="salary.no-wrap").text.replace(",", "")
        except:
            salary = "N/A"

        try:
            PostDate = each.find(class_="date").text.replace("\n", "").replace(",", "")
        except:
            PostDate = "N/A"
        if job_URL != "NULL":
            if checkifdup(job_URL) == True:
                print("Found new URL")
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
                    "posters": "ddd",
                    "jobSource": "Indeed",
                }
                r = json.dumps(body)
                mainPage = requests.post(url=techTrans, data=r, headers=header)
            else:
                print("Already found this URL")


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


def startup():
    start = time.time()

    print("Starting Download")
    Indeed()
    print("Completed Download")

    end = time.time()
    print(end - start)
