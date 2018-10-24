#	https://www.tensorflow.org/tutorials/representation/word2vec
#	tensorflow/examples/tutorials/word2vec/word2vec_basic.py
from __future__ import absolute_import
from __future__ import division
from __future__ import print_function

import collections
import math
import os
import sys
import argparse
import random
from tempfile import gettempdir
import zipfile

import numpy as np
from six.moves import urllib
from six.moves import xrange
import tensorflow as tf

from tensorflow.contrib.tensorboard.plugins import projector

dict_path="dict.txt"
input_file_name="total.wbr"
imageName="total.png"
similar_path="similar.txt"
test_path="StandardTest.txt"

def read_to_word(filename):
	with open(filename,'r',encoding='UTF-8') as f:
		for line in f:
			for word in line.split():
				yield word

vocabulary=list(read_to_word(input_file_name))

#	Build dictionary, replace rare words with UNK.
vocabulary_size=50000
num_steps=200001

def build_dataset(words, n_words):
	count=[['UNK',-1]]
	count.extend(collections.Counter(words).most_common(n_words-1))
	dictionary=dict()
	for word, _ in count:
		dictionary[word]=len(dictionary)
	data=list()
	for word in words:
		index=dictionary.get(word,0)
		data.append(index)
	return data

def build_dict(dict_path):
	with open(dict_path,'r',encoding='UTF-8') as f:
		dictionary={}
		reversed_dictionary={}
		for line in f:
			word=line.split()[0]
			code=int(line.split()[1])
			dictionary[word]=code
			reversed_dictionary[code]=word
	return dictionary, reversed_dictionary

#	data: code of the words.
data=build_dataset(vocabulary,vocabulary_size)
dictionary, reversed_dictionary=build_dict(dict_path)



with open("dict.txt","w",encoding='UTF-8') as f:
	for key in dictionary:
		f.write("{}\t{}\n".format(key,dictionary[key]))

		
data_index=0

#	Generate training batch for the skip gram model.
def generate_batch(batch_size, num_skips, skip_window):
	global data_index
	assert batch_size % num_skips ==0
	assert num_skips<=2*skip_window
	batch=np.ndarray(shape=(batch_size),dtype=np.int32)
	labels=np.ndarray(shape=(batch_size,1),dtype=np.int32)
	span=2*skip_window+1
	buffer=collections.deque(maxlen=span)
	if(data_index+span>len(data)):
		data_index=0
	buffer.extend(data[data_index:data_index+span])
	data_index+=span
	for i in range(batch_size//num_skips):
		context_words=[w for w in range(span) if w!=skip_window]
		words_to_use=random.sample(context_words,num_skips)
		for j,context_word in enumerate(words_to_use):
			batch[i*num_skips+j]=buffer[skip_window]
			labels[i*num_skips+j,0]=buffer[context_word]
		if data_index == len(data):
			buffer.extend(data[0:span])
			data_index=span
		else:
			buffer.append(data[data_index])	
			data_index+=1
	data_index=(data_index+len(data)-span)%len(data)
	return batch, labels

batch, labels=generate_batch(batch_size=8,num_skips=2,skip_window=1)

batch_size=128
embedding_size=128
skip_window=1
num_skips=2
num_sampled=64

valid_size=10
valid_window=100

def read_test(test_path):
	with open(test_path,'r', encoding= 'UTF-8') as f:
		for line in f:
			yield dictionary[line[:-1]]

ex_list=list(read_test(test_path))
valid_size=len(ex_list)
ex=np.array(ex_list,dtype=np.int32)
graph=tf.Graph()

with graph.as_default():
	train_inputs=tf.placeholder(tf.int32,shape=[batch_size])
	train_labels=tf.placeholder(tf.int32,shape=[batch_size,1])
	valid_dataset=tf.constant(ex,dtype=tf.int32)

	with tf.device('/cpu:0'):
		with tf.name_scope('embeddings'):
			embeddings=tf.Variable(tf.random_uniform([vocabulary_size,embedding_size],-1.0,-1.0))
		embed=tf.nn.embedding_lookup(embeddings,train_inputs)

	with tf.name_scope('weight'):
		nce_weights=tf.Variable(tf.truncated_normal([vocabulary_size,embedding_size],stddev=1.0/math.sqrt(embedding_size)))
	with tf.name_scope('biases'):
		nce_biases=tf.Variable(tf.zeros([vocabulary_size]))

	with tf.name_scope('loss'):
		loss=tf.reduce_mean(
			tf.nn.nce_loss(
				weights=nce_weights,
				biases=nce_biases,
				labels=train_labels,
				inputs=embed,
				num_sampled=num_sampled,
				num_classes=vocabulary_size
		))
	tf.summary.scalar('loss',loss)

	with tf.name_scope('optimizer'):
		optimizer=tf.train.GradientDescentOptimizer(0.2).minimize(loss)
	
	norm=tf.sqrt(tf.reduce_sum(tf.square(embeddings),1,keepdims=True))
	normalized_embeddings=embeddings/norm
	valid_embeddings=tf.nn.embedding_lookup(normalized_embeddings,valid_dataset)

	similarity=tf.matmul(valid_embeddings,normalized_embeddings,transpose_b=True)

	merged=tf.summary.merge_all()

	init=tf.global_variables_initializer()

	saver=tf.train.Saver()


with tf.Session(graph=graph) as session:
	init.run()

	print('Initialized')

	average_loss=0

	for step in xrange(num_steps):
		batch_inputs, batch_labels=generate_batch(batch_size,num_skips,skip_window)
		feed_dict={train_inputs:batch_inputs,train_labels:batch_labels}
		run_metadata=tf.RunMetadata()	

		_, summary, loss_val=session.run(
			[optimizer, merged, loss],
			feed_dict=feed_dict,
			run_metadata=run_metadata
		)
		average_loss+=loss_val

		if step%2000==0:
			if(step>0):
				average_loss/=2000
			print('Average loss at step ', step, ': ', average_loss)
			average_loss=0

	results=[]
	final_sim=similarity.eval()

	for i in xrange(valid_size):
		valid_word=reversed_dictionary[ex[i]]
		top_k=20
		nearest=(-final_sim[i,:]).argsort()[1:top_k+1]
		similars= list( map(lambda x:final_sim[i,x],nearest))
		line=valid_word
		for k in xrange(top_k):
			close_word=reversed_dictionary[nearest[k]]
			line="%s\t%s %s"%(line,close_word, similars[k])
		results.append(line)

	with open(similar_path,'w+',encoding="UTF-8") as f:
		for result in results:
			f.write("%s\n"%result)
	f.close()
