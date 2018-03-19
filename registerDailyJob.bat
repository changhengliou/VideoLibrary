schtasks /create /tn cns_daily_job /tr C:\project\PointVideoGallery\PointVideoGallery\bin\ScheduleTaskExecuter.exe /sc DAILY /st 00:05 /ru "System"
