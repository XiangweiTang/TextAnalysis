import RestoreModel as rm
import sys
input_path=sys.argv[1]
output_path=sys.argv[2]

rm.predict(input_path,"PosNeg_model.h5",output_path)