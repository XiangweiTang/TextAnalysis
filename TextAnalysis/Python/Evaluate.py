import RestoreModel as rm
import sys

data_path=sys.argv[1]
label_path=sys.argv[2]
output_path=sys.argv[3]

rm.evaluate(data_path,label_path,"PosNeg_model.h5",output_path)