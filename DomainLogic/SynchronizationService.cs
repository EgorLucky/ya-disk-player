﻿using DomainLogic.Entities;
using DomainLogic.ResponseModels;
using DomainLogic.YandexApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public class SynchronizationService
    {
        private readonly ISynchronizationHistoryRepository _repository;
        private readonly ISynchronizationMessageService _messageService;
        private readonly IYandexDiskApi _yandexDiskApi;

        public SynchronizationService(
            ISynchronizationHistoryRepository repository, 
            ISynchronizationMessageService messageSevice,
            IYandexDiskApi yandexDiskApi)
        {
            _repository = repository;
            _messageService = messageSevice;
            _yandexDiskApi = yandexDiskApi;
        }

        public async Task<SynchronizationStartResponseModel> Start(string yandexId, string accessToken, string refreshToken)
        {
            var unfinishedProcess = await _repository.GetRunningProcess(yandexId);

            if (unfinishedProcess != null)
                return new SynchronizationStartResponseModel(
                    Success: false,
                    ErrorMessage: $"Running synchronization process already exists"
                    );

            var synchProcess = new SynchronizationProcess(
                Id: Guid.NewGuid(),
                CreateDateTime: DateTimeOffset.Now,
                YandexUserId: yandexId
                );
            await _repository.Add(synchProcess);

            await _messageService.Send(synchProcess.Id, accessToken, refreshToken);

            return new SynchronizationStartResponseModel(
                Success: true,
                SynchronizationProcessId: synchProcess.Id
            );
        }


    }
}
