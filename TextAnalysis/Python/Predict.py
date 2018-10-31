import RestoreModel as rm
import sys
input_path=sys.argv[1]
model_path=sys.argv[2]
output_path=sys.argv[3]

rm.predict(input_path,model_path,output_path)