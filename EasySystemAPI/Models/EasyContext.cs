using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class EasyContext : DbContext
    {
        public EasyContext(DbContextOptions options) : base(options) { }

        public DbSet<Users> users { get; set; }
        public DbSet<UserSignUpCode> userSignUps { get; set; }
        public DbSet<Country> countries { get; set; }
        public DbSet<Bank> banks { get; set; }
        public DbSet<UserBankInfo> userBankInfos { get; set; }
        public DbSet<UserSkills> userSkills { get; set; }
        public DbSet<UserWhiteBoard> userWhiteBoards { get; set; }
        public DbSet<Transections> transections { get; set; }
        public DbSet<UserLostTrainee>  userLostTrainees { get; set; }
        public DbSet<UserDrawRequest>  userDrawRequests { get; set; }
        public DbSet<UserPayment>  userPayments { get; set; }
        public DbSet<Coupan>  coupans { get; set; }
        public DbSet<UserHelpingMaterial> userHelpingMaterial { get; set; }
        public DbSet<Blog> blogs { get; set; }
        public DbSet<UserVerification> userVerifications { get; set; }
        public DbSet<UserVerificationType>  userVerificationTypes { get; set; }
        public DbSet<InfoSend> infoSends { get; set; }
        public DbSet<InfoSend_Company> infoSend_Companies  { get; set; }
        public DbSet<SkillType> skillTypes  { get; set; }
        public DbSet<SkillMaterial> skillMaterials  { get; set; }
        public DbSet<SkillMaterialDetail> skillMaterialDetails  { get; set; }
        public DbSet<Question_Story> question_Stories  { get; set; }
        public DbSet<UserQuestionnaireResult> userQuestionnaireResults  { get; set; }
        public DbSet<UserCertificate> userCertificates  { get; set; }
        public DbSet<UserMentors> UserMentors { get; set; }
        public DbSet<FeeBreakup> feeBreakups { get; set; }
        public DbSet<MentorFee> mentorFees { get; set; }
        public DbSet<LoginLogs> loginLogs { get; set; }

        //PublicLayout
        public DbSet<FeeList> feeLists { get; set; }
        public DbSet<TrainingInfo> trainingInfos { get; set; }
        public DbSet<TrainingType> trainingTypes { get; set; }
    }
}
