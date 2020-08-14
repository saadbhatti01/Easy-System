using EasySystem.EasyAPI;
using EasySystemAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EasySystem.General
{
    public class Common
    {
        EasySysAPI _api = new EasySysAPI();

        public string GetSerialNo()
        {
            var SerialNo = "";
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/GetSerialNo");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                SerialNo = JsonConvert.DeserializeObject<string>(res);
            }
            return SerialNo;
        }

        public SkillsFee GetSkillsFee()
        {
            SkillsFee fee = new SkillsFee();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/GetFeeAmount");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                fee = JsonConvert.DeserializeObject<SkillsFee>(res);
            }
            return fee;
        }

        public Users GetUser(int id)
        {
            Users users = new Users();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Users/GetUser?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<Users>(res);
            }
            return users;
        }

        public Users GetMentor(int id)
        {
            Users users = new Users();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Users/GetRefData?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<Users>(res);
            }
            return users;
        }

        public UserSignUpCode PhoneNoWithCountryCode(EasySystem.Models.PhoneNumber number)
        {
            UserSignUpCode users = new UserSignUpCode();
            HttpClient client = _api.Initial();
            var data = client.PostAsJsonAsync("UserSignUpCodes/GetPhoneNoWithCountryCode", number);
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<UserSignUpCode>(res);
            }
            return users;
        }

        public string CountryName(int id)
        {
            var CountryName = "";
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Users/GetCountryName?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                CountryName = res;
            }
            return CountryName;
        }


        public List<SkillType> GetSkillTypes()
        {
            List<SkillType> sList = new List<SkillType>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Questionnaire/GetSkillType");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                sList = JsonConvert.DeserializeObject<List<SkillType>>(res);
            }
            return sList;
        }

        public List<EasySystem.Models.QuestionVM> GetQuestions()
        {
            List<EasySystem.Models.QuestionVM> sList = new List<EasySystem.Models.QuestionVM>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Questionnaire/GetQuestions");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                sList = JsonConvert.DeserializeObject<List<EasySystem.Models.QuestionVM>>(res);
            }
            return sList;
        }


        public List<SkillType> GetSkills()
        {
            List<SkillType> sList = new List<SkillType>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Questionnaire/GetSkills");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                sList = JsonConvert.DeserializeObject<List<SkillType>>(res);
            }
            return sList;
        }

        public List<SkillType> GetTestSkills(int id)
        {
            List<SkillType> sList = new List<SkillType>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Questionnaire/GetTestSkills?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                sList = JsonConvert.DeserializeObject<List<SkillType>>(res);
            }
            return sList;
        }

        public List<Question_Story> GetTestQuestions(EasySystem.Models.UserSkills user)
        {
            List<Question_Story> sList = new List<Question_Story>();
            HttpClient client = _api.Initial();
            var data = client.PostAsJsonAsync("Questionnaire/GetTestQuestions", user);
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                sList = JsonConvert.DeserializeObject<List<Question_Story>>(res);
            }
            return sList;
        }

        public List<Question_Story> TestVerification(Question_Story qData)
        {
            List<Question_Story> sList = new List<Question_Story>();
            HttpClient client = _api.Initial();
            var data = client.PostAsJsonAsync("Questionnaire/TestVerification", qData);
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                sList = JsonConvert.DeserializeObject<List<Question_Story>>(res);
            }
            return sList;
        }

        public bool CheckNumber(string Num)
        {
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Questionnaire/CheckNumber?=No" + Num);
            data.Wait(); 
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CheckTest(int id)
        {
            int No = 0;
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Questionnaire/CheckTest?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                No = JsonConvert.DeserializeObject<int>(res);
            }
            return No;
        }

        public string RandomString(int length)
        {
            const string chars = "123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //For PubliceAdmin Portal

        public int GetTrainingTypeCount()
        {
            int SerialNo = 0;
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Public/TrainingTypeCount");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                SerialNo = JsonConvert.DeserializeObject<int>(res);
            }
            return SerialNo;
        }

        public int GetTrainingInfoCount()
        {
            int SerialNo = 0;
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Public/GetTrainingInfoCount");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                SerialNo = JsonConvert.DeserializeObject<int>(res);
            }
            return SerialNo;
        }
    }
}
