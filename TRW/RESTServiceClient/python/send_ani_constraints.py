# -*- coding: utf-8 -*-
"""
Created on Fri Jul 10 15:20:26 2015

@author: xili01
"""

import urllib2, json

def _create_input(constraint_file_name):
    with open(constraint_file_name) as constraint_file:
        constraint_dict = json.load(constraint_file, strict=False)
    mg_input_dict = {
	  "aniConstranitsName": constraint_file_name,
	  "aniConstranitsContent":constraint_dict,
	  "trajectoryMarkerURL": "resources/models/sphere/blue_sphere.xml",
	  "keyframeMarkerURL": "resources/models/sphere/red_sphere.xml"
    }
    return mg_input_dict

def send_ani_constraints():
    """ Based on the example from http://isbullsh.it/2012/06/Rest-api-in-python
    """

    mg_server_url = 'http://localhost:8081/BVH/SetAnimationConstraints'	
    constraint_file_name = 'ConstraintsModel.json'
    mg_input_dict = _create_input(constraint_file_name)
    mg_input_string = json.dumps(mg_input_dict)
    request = urllib2.Request(mg_server_url, mg_input_string)
    print "send message and wait for answer..."
    handler = urllib2.urlopen(request)
    result = handler.read()
    print result
    
if __name__ == "__main__":
    send_ani_constraints()
  
