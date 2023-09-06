# Update_YT_channel_Automatically
- Pipeline schedule: 0 8 * * *
  script:
  `build 'Step1-Get_Ghost_Story_MP3'`
  `build 'Step2-Make_Ghost_Story_Video'`
  `build 'Step3_Upload_To_YT_channel'`

  
- Run selenium test:
  `cd C:\Program Files (x86)\NUnit.org\nunit-console\nunit3-console --test=Tests.GhostStoryGenerator C:\Users\mxie\source\repos\UIAutomationTestProjects\UIAutomationTest\bin\Debug\net472\UIAutomationTest.dll`
  `cd C:\Program Files (x86)\NUnit.org\nunit-console\nunit3-console --test=Tests.GhostStoryPoster --params:mp4_filename="GhostStory_en.mp4" --params:mp4_filename="GhostStory_zn.mp4" C:\Users\mxie\source\repos\UIAutomationTestProjects\UIAutomationTest\bin\Debug\net472\UIAutomationTest.dll`

  
- Combine audio and video together
  `cd c:\Users\mxie\Downloads`
  `dir *.mp3 | sort LastWriteTime | dir -name > temp1.txt`

  `Get-Content 'C:\Users\mxie\Downloads\temp1.txt' -tail 2 > temp2.txt`

  `$mp3_filename_en = Get-Content 'C:\Users\mxie\Downloads\temp2.txt' -tail 1`
  `echo $mp3_filename_en`
  `$mp3_filename_zn = Get-Content 'C:\Users\mxie\Downloads\temp2.txt' -first 1`
  `echo $mp3_filename_zn`

  `cp $mp3_filename_en c:\ghost_story\$mp3_filename_en`
  `$title_en = $mp3_filename_en.Replace(".mp3", "")`
  `echo $title_en`

  `cp $mp3_filename_zn c:\ghost_story\$mp3_filename_zn`
  `$title_zn = $mp3_filename_zn.Replace(".mp3", "")`
  `echo $title_zn`

  `cd c:\ghost_story\`
  `##c:\ffmpeg\bin\ffmpeg.exe -i host.mp4 -i $mp3_filename_en -c:v copy -map 0:v -map 1:a -y $final_filename`
  `##fmpeg -ss 00:10:45 -i input.avi -c:v libx264 -crf 18 -to 00:11:45 -c:a copy output.mp4`

  `c:\ffmpeg\bin\ffmpeg.exe -i host.mp4  -i $mp3_filename_en -c:v  copy -map 0:v -map 1:a -y GhostStory_en.mp4`
  `echo $title_en > title_en.txt`

  `c:\ffmpeg\bin\ffmpeg.exe -i host.mp4  -i $mp3_filename_zn -c:v  copy -map 0:v -map 1:a -y GhostStory_zn.mp4`
  `echo $title_zn > title_zn.txt`

