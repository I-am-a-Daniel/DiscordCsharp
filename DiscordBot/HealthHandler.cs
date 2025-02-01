using System;

public class HealthHandler{
    public static string CheckBMI(double height, double weight){
        if(height < 150 || height > 220 || weight < 40 || weight > 200){
            return "Nem számolható BMI a megadott adatok alapján.";
        }
        height = height / 100;
        double bmi = Math.Round(weight / (height * height), 1);
        string bmiEval;
        if(bmi < 18.5){
            bmiEval = "Alultáplált";
        }else if(bmi >= 18.5 && bmi < 24.9){
            bmiEval = "Átlagos";
        }else if(bmi >= 25 && bmi < 29.9){
            bmiEval = "Túlsúlyos";
        }else if(bmi >= 30 && bmi < 34.9){
            bmiEval = "Elhízott";
        }else if(bmi >= 35 && bmi < 39.9){
            bmiEval = "Súlyosan elhízott";
        }else{
            bmiEval = "Veszélyesen elhízott";
        }
        return ($"BMI: {bmi} - {bmiEval}");
    }
}