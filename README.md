
var query = context.EMPL_AccountsIDs
                // First Right Join: EMPL_AccountsIDs to ADMIN_LoginStatusValues
                .GroupJoin(
                    context.ADMIN_LoginStatusValues,
                    emplAcc => emplAcc.FK_Status_PK,
                    adminLogin => adminLogin.LStat_PK,
                    (emplAcc, adminGroup) => new { emplAcc, adminGroup }
                )
                .SelectMany(
                    x => x.adminGroup.DefaultIfEmpty(),
                    (x, adminLogin) => new { x.emplAcc, adminLogin }
                )
                // Second Right Join: EMPL_AccountsIDs to EMPL_Data
                .GroupJoin(
                    context.EMPL_Data,
                    x => x.emplAcc.DataId,
                    emplData => emplData.Emp_PK,
                    (x, dataGroup) => new { x.emplAcc, x.adminLogin, dataGroup }
                )
                .SelectMany(
                    x => x.dataGroup.DefaultIfEmpty(),
                    (x, emplData) => new { x.emplAcc, x.adminLogin, emplData }
                )
                // Left Join: EMPL_Data to ADMIN_Systems
                .GroupJoin(
                    context.ADMIN_Systems,
                    x => x.emplData.Emp_PK,
                    adminSys => adminSys.Sys_PK,
                    (x, adminSysGroup) => new { x.emplAcc, x.adminLogin, x.emplData, adminSysGroup }
                )
                .SelectMany(
                    x => x.adminSysGroup.DefaultIfEmpty(),
                    (x, adminSys) => new { x.emplAcc, x.adminLogin, x.emplData, adminSys }
                )
                // Add an OrderBy clause
                .OrderBy(result => result.adminSys.SomeProperty);

            // Execute the query
            var results = query.ToList();
