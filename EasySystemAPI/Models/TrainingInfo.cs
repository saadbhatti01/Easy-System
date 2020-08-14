using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("TrainingInfo")]
    public class TrainingInfo
    {
        [Key]
        public int tiId { get; set; }
        public string tiName { get; set; }
        public string tiDetail { get; set; }
        public string tiDuration { get; set; }
        public double tiPrice { get; set; }
        public string tiPriceDuration { get; set; }
        public string tiImage { get; set; }
        public string tiCoverPage { get; set; }
        public string tiStatus { get; set; }
        public int ttId { get; set; }
        public virtual TrainingType GetTrainingType { get; set; }
    }

    public class TrainingInfoVM
    {
        public int tiId { get; set; }
        public string tiName { get; set; }
        public string tiDetail { get; set; }
        public string tiDuration { get; set; }
        public double tiPrice { get; set; }
        public string tiPriceDuration { get; set; }
        public string tiImage { get; set; }
        public string tiCoverPage { get; set; }
        public string tiStatus { get; set; }
        public int ttId { get; set; }
        public string ttName { get; set; }
    }

    [Table("TrainingType")]
    public class TrainingType
    {
        [Key]
        public int ttId { get; set; }
        public string ttName { get; set; }
        public string ttStatus { get; set; }
    }

    [Table("FeeList")]
    public class FeeList
    {
        [Key]
        public int fLId { get; set; }
        public string flTraineeName { get; set; }
        public string flTraineeSurName { get; set; }
        public string flTraineeEmail { get; set; }
        public string flTraineeContact { get; set; }
        public string flTraineeComment { get; set; }
        public int tiId { get; set; }
        public string flStatus { get; set; }
        public DateTime flCreatedDate { get; set; }
        public DateTime trainingCompletedDate { get; set; }
        public string RemarksByAdmin { get; set; }

        public virtual TrainingInfo GetTrainingInfo { get; set; }

    }
    public class FeeListVM
    {
        [Key]
        public int fLId { get; set; }
        public string flTraineeName { get; set; }
        public string flTraineeSurName { get; set; }
        public string flTraineeEmail { get; set; }
        public string flTraineeContact { get; set; }
        public string flTraineeComment { get; set; }
        public int tiId { get; set; }
        public string flStatus { get; set; }
        public DateTime flCreatedDate { get; set; }
        public DateTime trainingCompletedDate { get; set; }
        public string RemarksByAdmin { get; set; }

        public double tiPrice { get; set; }

    }

    public class JazzCash
    {
        public int UsrId { get; set; }
        public string ResponseCode { get; set; }

        public string pp_ResponseCode { get; set; }

        public string pp_ResponseMessage { get; set; }

        public string ResponseMessage { get; set; }

        public string pp_Version { get; set; }

        public string pp_IsRegisteredCustomer { get; set; }

        public string pp_TxnType { get; set; }

        public string pp_MerchantID { get; set; }

        public string pp_SubMerchantID { get; set; }

        public string pp_Language { get; set; }

        public string pp_TxnRefNo { get; set; }

        public string pp_RetreivalReferenceNo { get; set; }

        public string pp_TxnCurrency { get; set; }

        public string pp_Amount { get; set; }

        public string pp_DiscountedAmount { get; set; }

        public string pp_DiscountBank { get; set; }

        public string pp_BankID { get; set; }

        public string pp_ProductID { get; set; }

        public string pp_TxnDateTime { get; set; }

        public string pp_TxnExpiryDateTime { get; set; }

        public string pp_BillReference { get; set; }

        public string pp_Description { get; set; }


        public string pp_CustomerID { get; set; }


        public string pp_CustomerEmail { get; set; }


        public string pp_CustomerMobile { get; set; }


        public string pp_TokenizedCardNumber { get; set; }


        public string pp_CustomerCardNo { get; set; }


        public string pp_CustomerCardExpiry { get; set; }


        public string pp_TokenizationResponseCode { get; set; }


        public string pp_TokenizationResponseMsg { get; set; }


        public string ppmpf_1 { get; set; }


        public string ppmpf_2 { get; set; }


        public string ppmpf_3 { get; set; }


        public string ppmpf_4 { get; set; }


        public string ppmpf_5 { get; set; }


        public string ppmpf_6 { get; set; }


        public string ppmbf_1 { get; set; }


        public string ppmbf_2 { get; set; }


        public string ppmbf_3 { get; set; }


        public string ppmbf_4 { get; set; }


        public string ppmbf_5 { get; set; }


        public string ppmbf_6 { get; set; }

        public string pp_SecureHash { get; set; }

        public string pp_Password { get; set; }

        public string pp_ReturnURL { get; set; }

        public string salt { get; set; }
    }
}
