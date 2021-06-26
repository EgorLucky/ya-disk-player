﻿using DomainLogic.Entities;
using DomainLogic.Repositories;
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
                CreateDateTime: DateTimeOffset.Now,
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
                CreateDateTime: DateTimeOffset.Now
            );

            await _repository.AddCancellation(processCancellation);

            return new SynchronizationStopResponseModel(Success: true);
        }
    }
}
