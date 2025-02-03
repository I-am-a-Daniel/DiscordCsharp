using Discord;
using System;

public class HealthHandler{
    public static EmbedBuilder CheckBMI(double height, double weight){
        if(height < 150 || height > 220 || weight < 40 || weight > 200){
            return new EmbedBuilder().WithDescription("Nem számolható BMI a megadott adatok alapján.");
        }
        int heightcm = (int)height;
        height = height / 100;
        double bmi = Math.Round(weight / (height * height), 1);
        string bmiEval;
        Color embedColor;

        if(bmi < 18.5){
            bmiEval = "Alultáplált";
            embedColor = Color.Blue;
        }else if(bmi >= 18.5 && bmi < 24.9){
            bmiEval = "Átlagos";
            embedColor = Color.Green;
        }else if(bmi >= 25 && bmi < 29.9){
            bmiEval = "Túlsúlyos";
            embedColor = Color.Orange;
        }else if(bmi >= 30 && bmi < 34.9){
            bmiEval = "Elhízott";
            embedColor = Color.Red;
        }else if(bmi >= 35 && bmi < 39.9){
            bmiEval = "Súlyosan elhízott";
            embedColor = Color.DarkRed;
        }else{
            bmiEval = "Veszélyesen elhízott";
            embedColor = Color.DarkRed;
        }

        var response = new EmbedBuilder()
            .WithTitle($"{bmiEval}")
            .WithDescription($"A BMI-d: {bmi}\nIdeális testsúly: {CalculateIdealWeight(heightcm)[0]}-{CalculateIdealWeight(heightcm)[1]} kg.")
            .WithFooter($"Magasság: {heightcm} cm | Testsúly: {weight} kg")
            .WithColor(embedColor);
        return response;
    }

    public static int[] CalculateIdealWeight(double height){
        if(height < 150 || height > 220){
            return new int[]{-1, -1};                                       //TODO: Error handling
        }
        height = height / 100;
        int minWeight = (int)Math.Round(18.5 * height * height);
        int maxWeight = (int)Math.Round(24.9 * height * height);
        return new int[]{minWeight, maxWeight};
    }
}