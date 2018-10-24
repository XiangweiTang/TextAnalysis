import io
import numpy as np

def create_data_itor(file_path):
	with open(file_path,'r',encoding='UTF-8') as f:		
		for line in f:
			split=line.split(' ')
			yield list(map(lambda x:int(x),split))

def create_data(file_path):
	itor=create_data_itor(file_path)
	return np.array(list(itor))

def create_label_itor(file_path):
	with open(file_path,'r',encoding='UTF-8') as f:
		for line in f:
			yield int(line)

def create_label(file_path):
	itor=create_label_itor(file_path)
	return np.array(list(itor))

def create_dict(file_path):
	dictionary={}
	with open(file_path,'r',encoding='UTF-8') as f:
		for line in f:
			key=line.split('\t')[0]
			value=int(line.split('\t')[1])+1
			dictionary[key]=value
	dictionary['<UNK>']=1
	dictionary['<PAD>']=0
	return dictionary