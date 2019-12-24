
from flask import Flask,request,jsonify

import nltk
import secrets
import NLTKProcessor

app = Flask(__name__)


@app.before_first_request
def downloadNLTK():
    nltk.download('popular')

@app.route("/")
def hello():
    return "I am here to Classify"

@app.route("/extract_keyphrases_from_text",methods=["POST"])
def extract_keyphrases_from_text():
    try:
        authKey = request.json["authKey"]
        if authKey == secrets.AUTHKEYPrime:
            final = NLTKProcessor.extractKeyPhrasesFromText(request.json["textIn"])
            return jsonify({"rank_list":final})
        else:
            return 'Processing Error Occured',500
    except:
        return 'Processing Error Occured',500

@app.route("/extract_summary_from_text",methods=["POST"])
def extract_summary_from_text():
    try:
        authKey = request.json["authKey"]
        if authKey == secrets.AUTHKEYPrime:
            final = NLTKProcessor.generate_summary(request.json["textIn"])
            return jsonify({"SummaryText":final})
        else:
            return 'Processing Error Occured',500
    except:
        return 'Processing Error Occured',500
    