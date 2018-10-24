import tensorflow as tf
from tensorflow import keras
import DataPrepare as dp
import sys
import io

def evaluate(test_data_path, test_label_path, model_path, output_path, pad_index=0, max_length=256):
	test_data=dp.create_data(test_data_path)
	test_data=keras.preprocessing.sequence.pad_sequences(test_data,value=pad_index,padding="post",maxlen=max_length)
	test_labl=dp.create_label(test_label_path)
	model=keras.models.load_model(model_path)

	result=model.evaluate(test_data, test_labl)
	with open(output_path,'w+',encoding='UTF-8') as f:
		f.write(result)
	f.close()

def predict(test_data_path, model_path, output_path, pad_index=0, max_length=256):
	test_data=dp.create_data(test_data_path)
	test_data=keras.preprocessing.sequence.pad_sequences(test_data, value=pad_index, padding="post", maxlen=max_length)

	model=keras.models.load_model(model_path)

	results=model.predict(test_data)
	with open(output_path, 'w+', encoding='UTF-8') as f:
		for result in results:
			f.write("{}\n".format(result))
		f.close()
