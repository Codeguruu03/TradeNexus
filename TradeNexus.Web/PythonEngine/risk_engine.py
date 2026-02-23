import sys
import json

def calculate_risk(data):
    total_exposure = sum(t["quantity"] * t["price"] for t in data["trades"])
    available_margin = data["availableMargin"]

    required_margin = total_exposure * 0.2
    risk_ratio = required_margin / available_margin if available_margin > 0 else 1

    if risk_ratio > 0.8:
        risk_level = "High"
    elif risk_ratio > 0.5:
        risk_level = "Medium"
    else:
        risk_level = "Low"

    return {
        "totalExposure": total_exposure,
        "requiredMargin": required_margin,
        "riskLevel": risk_level,
        "limitBreach": required_margin > available_margin
    }

if __name__ == "__main__":
    input_data = json.loads(sys.argv[1])
    result = calculate_risk(input_data)
    print(json.dumps(result))