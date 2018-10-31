import RestoreModel as rm
import sys

data_path=sys.argv[1]
label_path=sys.argv[2]
model_path=sys.argv[3]
output_path=sys.argv[4]

rm.evaluate(data_path,label_path,model_path,output_path)