using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Utility;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Syntaq.Falcon.Submissions
{
    public class SubmissionManager : FalconDomainServiceBase
    {
        public IAbpSession _abpSession { get; set; }

        private readonly EntityManager _entityManager;
        private readonly IRepository<Apps.App, Guid> _appRepository;
        private readonly IRepository<Form, Guid> _formRepository;
        private readonly IRepository<Submission, Guid> _submissionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public SubmissionManager(EntityManager entityManager, IRepository<Apps.App, Guid> appRepository, IRepository<Form, Guid> formRepository, IRepository<Submission, Guid> submissionRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _entityManager = entityManager;
            _appRepository = appRepository;
            _formRepository = formRepository;
            _submissionRepository = submissionRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        private async Task CreateSubmission(Submission Submission)
        {
            if (_abpSession.TenantId != null)
            {
                Submission.TenantId = _abpSession.TenantId;
            }
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                await _submissionRepository.InsertAsync(Submission);
                //CurrentUnitOfWork.SaveChanges();
                unitOfWork.Complete();
            }
        }

        public virtual async Task<Submission> CreateAndOrFetchSubmission(CreateOrEditSubmissionDto SubmissionDto)
        {

            Apps.App app;
            Form form = new Form();
            Submission Submission = new Submission();

            if (_entityManager.IsSoftDelete(SubmissionDto.Id, null, EntityManager.SoftDeleteEntities.Submission))
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    Submission = _submissionRepository.FirstOrDefault((Guid)SubmissionDto.Id);
                    Submission.IsDeleted = false;
                    Submission.DeleterUserId = null;
                    Submission.DeletionTime = null;
                    _submissionRepository.Update(Submission);
                    //CurrentUnitOfWork.SaveChanges();
                }
            }

            if (SubmissionDto.FormId != null)
            {
                form = _formRepository.FirstOrDefault((Guid)SubmissionDto.FormId);
            }
            else
            {
                app = _appRepository.FirstOrDefault((Guid)SubmissionDto.AppId);
            }

            Submission = ObjectMapper.Map<Submission>(SubmissionDto);
            if (SubmissionDto.Id.ToString() == "00000000-0000-0000-0000-000000000000" || !_submissionRepository.GetAll().Any(i => i.Id == SubmissionDto.Id))
            {
                if (!string.IsNullOrEmpty(form.Id.ToString()) && form.Id.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    Submission.RequiresPayment = form.PaymentEnabled;
                    Submission.PaymentAmount = form.PaymentAmount;
                    Submission.AppId = null;
                    Submission.SubmissionStatus = "Started";
                }
                else
                {
                    Submission.FormId = null;
                }
                await CreateSubmission(Submission);
                //CurrentUnitOfWork.SaveChanges();
            }

            CurrentUnitOfWork.SaveChanges();
            Submission = _submissionRepository.Get(Submission.Id);
            return Submission;
        }

        public Submission FetchSubmissionByRecordMatterId(EntityDto<Guid> input)
        {
            Submission Submission = _submissionRepository.GetAll().Where(i => i.RecordMatterId == input.Id).FirstOrDefault();
            return Submission;
        }

        public virtual async void UpdateSubmission(CreateOrEditSubmissionDto SubmissionDto)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {

                    CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);
                    Submission Submission;

                    if (!_submissionRepository.GetAll().Any(i => i.Id == SubmissionDto.Id))
                    {
                        Submission = ObjectMapper.Map<Submission>(SubmissionDto);
                        await CreateSubmission(Submission);
                    }
                    else
                    {

                        Submission = _submissionRepository.Get((Guid)SubmissionDto.Id);
                        Form Form = _formRepository.GetAll().FirstOrDefault(e => e.Id == (SubmissionDto.FormId ?? Submission.FormId));

                        //Calculate PaymentAmount, AmountPaid and VoucherAmount !!!!
                        // Do not calculate here done when charging a card
                        //var paymentAmount = Form == null ? 0 : Form?.PaymentAmount != null ? Form.PaymentAmount : 0.00m;
                        //var VoucherAmount = Submission.VoucherAmount ?? SubmissionDto.VoucherAmount;
                        //var AmountPaid = paymentAmount - VoucherAmount;

                        Submission = _submissionRepository.Get((Guid)SubmissionDto.Id);
                        Submission.TenantId = SubmissionDto.TenantId ?? null;
                        Submission.AccessToken = SubmissionDto.AccessToken ?? null;
                        Submission.RequiresPayment = Form == null ? false : (bool)Form?.PaymentEnabled ? true : false;
                        Submission.PaymentStatus = Submission.PaymentStatus ?? SubmissionDto.PaymentStatus;

                        // Do not calculate here done when charging a card !!!!
                        // Payment and Voucher Amounts are set in the payment api when charging a card and not here
                        Submission.PaymentAmount = Form == null ? 0 : Form?.PaymentAmount != null ? Form.PaymentAmount : 0.00m;
                        Submission.VoucherAmount = Submission.VoucherAmount ?? SubmissionDto.VoucherAmount;
                        Submission.AmountPaid = Submission.AmountPaid ?? SubmissionDto.AmountPaid;
                        

                        Submission.ChargeId = Submission.ChargeId ?? SubmissionDto.ChargeId;
                        Submission.SubmissionStatus = SubmissionDto.SubmissionStatus ?? Submission.SubmissionStatus;
                        Submission.RecordId = Submission.RecordId ?? SubmissionDto.RecordId;
                        Submission.RecordMatterId = Submission.RecordMatterId ?? SubmissionDto.RecordMatterId;
                        Submission.UserId = Submission.UserId ?? SubmissionDto.UserId;
                        Submission.AppId = Submission.AppId ?? (SubmissionDto.AppId != null && Convert.ToString(SubmissionDto.AppId) != "00000000-0000-0000-0000-000000000000" ? SubmissionDto.AppId : null);
                        Submission.AppJobId = Submission.AppJobId ?? SubmissionDto.AppJobId;
                        Submission.FormId = Submission.FormId ?? (SubmissionDto.FormId != null && Convert.ToString(SubmissionDto.FormId) != "00000000-0000-0000-0000-000000000000" ? SubmissionDto.FormId : null);
                        Submission.LastModificationTime = DateTime.UtcNow;
                        _submissionRepository.Update(Submission);
                    }
                    CurrentUnitOfWork.SaveChanges();

                    _submissionRepository.GetDbContext().Entry(Submission).State = EntityState.Detached;

                }
                unitOfWork.Complete();
            }
        }

        public virtual async void UpdateSubmissionStatus(CreateOrEditSubmissionDto SubmissionDto)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {

                CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);

                Submission Submission;
                if (!_submissionRepository.GetAll().Any(i => i.Id == SubmissionDto.Id))
                {
                    Submission = ObjectMapper.Map<Submission>(SubmissionDto);
                    Submission.VoucherAmount = SubmissionDto.VoucherAmount;

                    await CreateSubmission(Submission);
                }
                else
                {
                    Submission = _submissionRepository.Get((Guid)SubmissionDto.Id);
                    Submission.SubmissionStatus = SubmissionDto.SubmissionStatus;
                    // Submission.VoucherAmount = SubmissionDto.VoucherAmount;
                    //_submissionRepository.Update(Submission);
                    await _submissionRepository.UpdateAsync(Submission);
                }
                CurrentUnitOfWork.SaveChanges();
                _submissionRepository.GetDbContext().Entry(Submission).State = EntityState.Detached;

                unitOfWork.Complete();

            }
        }
    }
}