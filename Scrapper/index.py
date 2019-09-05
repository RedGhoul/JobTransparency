import requests
import bs4
from bs4 import BeautifulSoup
import pandas as pd
import time
from datetime import datetime
from threading import Thread
import os
import glob

max_results_per_city = 2000
postionFind = ["software+developer", "react+developer", "devops", "software+engineer"]
job_Type = "fulltime"
city_set = ["Ontario", "Vancouver"]
max_age = "15"
host = "ca.indeed.com"


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
    soup = BeautifulSoup(page.text, "html.parser", from_encoding="utf-8")
    for div in soup.find_all(class_="result"):
        for each in soup.find_all(class_="result"):
            try:
                title = each.find(class_="jobtitle").text.replace("\n", "").replace(",","")
            except:
                title = "NULL"
            try:
                job_URL = "https://" + host + each.find(class_="jobtitle")["href"].replace(",","")
            except:
                job_URL = "NULL"

            try:
                location = each.find("span", {"class": "location"}).text.replace(
                    "\n", ""
                ).replace(",","")
            except:
                location = "NULL"
            try:
                company = each.find(class_="company").text.replace("\n", "").replace(",","")
            except:
                company ="NULL"
            try:
                salary = each.find(class_="salary.no-wrap").text.replace(",","")
            except:
                salary = "NULL"
            try:
                synopsis = each.find(class_="summary").text.replace("\n", "").replace(",","")
            except:
                synopsis = "NULL"
            try:
                PostDate = each.find(class_="date").text.replace("\n", "").replace(",","")
            except:
                PostDate = "NULL"
            df_more = df_more.append(
                {
                    "Title": title,
                    "JobURL": job_URL,
                    "PostDate": PostDate,
                    "Location": location,
                    "Company": company,
                    "Salary": salary,
                    "Synopsis": synopsis,
                },
                ignore_index=True,
            )
    df_more.to_csv(finalFileName + ".csv", encoding="utf-8")


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

    extension = "csv"
    all_filenames = [i for i in glob.glob("*.{}".format(extension))]
    # combine all files in the list
    rays = []
    for x in all_filenames:
        newdata = pd.read_csv(x, index_col=[0])
        rays.append(newdata)
    print(len(rays))
    df_more = pd.concat(rays)
    df_more.reset_index(drop=True)
    # export to csv
    now = datetime.now()
    dt_string = now.strftime("%d/%m/%Y %H:%M:%S").replace(" ", "").replace(":", "")
    df_more.to_csv("IndeedJobDump"+dt_string+".csv", encoding="utf-8")

    print("Doing Clean Up")
    for x in all_filenames:
        os.remove(x)

    end = time.time()
    print(end - start)
