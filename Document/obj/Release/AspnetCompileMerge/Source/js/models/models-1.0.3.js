(function(window) {
    window.models = window.models || {};

    models.ExternalSource = function() {
        return {
            ID: 0,
            OrganizationID: 0,
            OrganizationName: '',
            CreationDate: '',
            Number: ''
        };
    };
    models.Destination = models.Source = function() {
        return {
            ID: 0,
            CitizenID: 0,
            OrganizationID: 0,
            DepartmentID: 0,
            WorkerID: 0,
            CreationDate: '',
            Number: '',
            OrganizationName: ''
        };
    };

    models.Document = function() {
        return {
            ID: 0,
            DepartmentID: 0,
            DocStatusID: 0,
            Number: '',
            CreationDate: '',
            CodeID: 0,
            CodeName: '',
            Files: [],
            Labels: [],
            Notes: '',
            ParentDocumentID: 0,
            DocStateID: 0
        };
    };

    models.DocTemplate = function() {
        return {
            ID: 0,
            DocumentID: 0,
            IsInput: true,
            Content: '',
            PublicContent: '',
            Changes: '',
            DocTypeID: 0,
            QuestionTypeID: 0,
            IsControlled: false,
            IsSpeciallyControlled: false,
            IsIncreasedControlled: false,
            IsPublic: true,
            NumberCopies: 0,
            HeadID: 0,
            WorkerID: 0,
            TemplateId: 3
        };
    };

    models.DocStatement = function () {
        return {
            ID: 0,
            DocumentID: 0,
            Branches: [],
            CitizenID: 0,
            Content: '',
            DeliveryTypeID: 0,
            HeadID: 0,
            InputDocTypeID: 0,
            InputMethodID: 0,
            InputSignID: 0,
            InputSubjectTypeID: 0,
            IsNeedAnswer: false,
            IsReception: false,
            TemplateId: 3
        };
    };

    models.Citizen = function () {
        return {
            ID: 0,
            LastName: '',
            FirstName: '',
            MiddleName: '',
            Address: '',
            PhoneNumber: '',
            CityObjectTypeShortName: '',
            CityObjectID: 0,
            CityObjectName: '',
            HouseNumber: '',
            Corps: '',
            ApartmentNumber: '',
            Work: '',
            Sex: 0,
            SocialStatusID: 0,
            SocialCategories: []
        };
    };


    models.ControlCard = function () {
        return {
            ID: 0,
            DocumentID: 0,

            CardNumber: 1,
            HeadID: 0,
            WorkerID: 0,
            ExecutiveDepartmentID: 0,
            DepartmentID: 0,

            StartDate: '',
            EndDate: '',
            Resolution: '',
            Notes: '',
            IsSpeciallyControlled: true,
            FixedWorkerID: 0,

            ControlResponseDate: '',
            ControlResponse: '',
            HeadResponseID: 0,

            DocStatusID: 0,
            CardStatusID: 1,
            CardStatus: { ID: 1, Name: 'СТОЇТЬ НА КОНТРОЛІ' },

            IsDecisionOfSession: false,
            IsDecisionOfExecutiveCommittee: false,
            IsOrderOfHeader: false,
            IsActionWorkPlan: false,
            IsSendResponse: false,
            IsSendInterimResponse: false,
            IsLeftToWorker: false,
            IsAcquaintedWorker: false,
            IsContinuation: false,
            ParentControlCardID: 0,
            InnerNumber: ''
        };
    };

})(window);