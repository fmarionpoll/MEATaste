# MeaTaste

<<<<<<< HEAD
TasteMEA 
Read H5 file from MaxWell Biosystems and visualize its content
=======
Read H5 file from MaxLab Live Software and visualize its content
>>>>>>> 6aca211b7f126a156f9708284a9b080112394347
.NET core program written in C# for Windows 10

---
Structure of MxW files

Datasets: 
-versions
-mxw_version
-hdf_version

Groups:
-notes
-data_store
-recording
-wellplate
-bits
-settings
- groups

Structure data_store: = enumerated groups
with datasets:
- well_id
- recording_id
- start_time
- stop_time

Structure settings:
- gain
- lsb
- sampling
- hpf
- spike_threshold
- mapping:	
	. channel
	. electrode number
	. x coord
	. y coord

Structure groups: contain subgroups (subset of recroded channels)
- routed = group always present
