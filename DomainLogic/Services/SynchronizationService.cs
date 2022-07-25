using DomainLogic.Entities;
using DomainLogic.Repositories;
using DomainLogic.RequestModels;
using DomainLogic.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public class SynchronizationService
    {
        private readonly ISynchronizationHistoryRepository _repository;
        private readonly ISynchronizationMessageService _messageService;

        public SynchronizationService(
            ISynchronizationHistoryRepository repository, 
            ISynchronizationMessageService messageSevice)
        {
            _repository = repository;
            _messageService = messageSevice;
        }

        public async Task<SynchronizationStartResponseModel> Start(string yandexUserId, string accessToken, string refreshToken)
        {
            var unfinishedProcess = await _repository.GetRunningProcess(yandexUserId);

            if (unfinishedProcess != null)
                return new SynchronizationStartResponseModel(
                    Success: false,
                    ErrorMessage: $"Running synchronization process already exists"
                    );

            var synchProcess = new SynchronizationProcess(
                Id: Guid.NewGuid(),
                CreateDateTime: DateTimeOffset.UtcNow,
                YandexUserId: yandexUserId
                );
            await _repository.Add(synchProcess);

            await _messageService.Send(synchProcess.Id, accessToken, refreshToken);

            return new SynchronizationStartResponseModel(
                Success: true,
                SynchronizationProcessId: synchProcess.Id
            );
        }

        public async Task<SynchronizationStopResponseModel> Stop(string yandexUserId, Guid? synchronizationProcessId)
        {
            if(synchronizationProcessId == null)
                return new SynchronizationStopResponseModel(
                    ErrorMessage: $"synchronizationProcessId is null"
                    );

            var unfinishedProcess = await _repository.GetProcessById(synchronizationProcessId.Value);

            if (unfinishedProcess == null || unfinishedProcess.YandexUserId != yandexUserId)
                return new SynchronizationStopResponseModel(
                    ErrorMessage: $"Running synchronization process not found"
                    );

            var processCancellation = new SynchronizationProcessUserCancellation(
                SynchronizationProcessId: synchronizationProcessId.Value,
                CreateDateTime: DateTimeOffset.UtcNow
            );

            await _repository.AddCancellation(processCancellation);

            return new SynchronizationStopResponseModel(Success: true);
        }

        public async Task<List<SynchronizationProcess>> Get(GetSynchronizationProcessesRequestModel request, string yandexUserId)
        {
            if (request.Page < 1)
                request = request with { Page = 1 };
            if (request.Take < 1)
                request = request with { Take = 20 };

            return await _repository.Get(request, yandexUserId);
        }
    }
}
