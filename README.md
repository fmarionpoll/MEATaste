# MeaTaste

Read H5 file from MaxLab Live Software and visualize its content
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
