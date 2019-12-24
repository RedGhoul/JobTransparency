from rake_nltk import Rake
from bs4 import BeautifulSoup

import nltk

from flask import Flask,request,jsonify
app = Flask(__name__)

import secrets
@app.before_first_request
def downloadNLTK():
    nltk.download('popular')

@app.route("/")
def hello():
    return "I am here to Classify"

@app.route("/Classify",methods=["POST"])
def classify():
    try:
        authKey = request.json["authKey"]
        if authKey == secrets.AUTHKEYPrime:
            soup = BeautifulSoup(request.json["textIn"])
            htmlFreeText = soup.get_text()
            htmlFreeText.replace("-","")
            r = Rake()
            r.extract_keywords_from_text(htmlFreeText)
            final = []
            for pair in r.rank_list:
                newDic = {}
                newDic["Affinty"] = pair[0]
                newDic["Text"] = pair[1]
                final.append(newDic)


            return jsonify({"rank_list":final})
        else:
            return 'Record not found',400
    except:
        return 'Record not found',400
    