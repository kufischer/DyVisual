# -*- coding: utf-8 -*-
"""
Created on Fri Jul 10 15:20:26 2015

@author: erhe01
"""

import urllib2, json

def _create_mg_input(bvh_file_name, action_file_name):
    with open(bvh_file_name) as bvh_file:
        bvh_string = bvh_file.read()
    with open(action_file_name) as action_file:
        action_dict = json.load(action_file, strict=False)
    mg_input_dict = {
	  "aniName": bvh_file_name,
	  "aniContent":bvh_string,
	  "targetAvatarURI":"resources/models/male/male.xml",
	  "avatarID":"0",
	  "configString":"start,action,"+action_file_name ,
      "actionName": action_file_name,
      "actionContent": action_dict
    }
    return mg_input_dict

def send_mg_input():
    """ Based on the example from http://isbullsh.it/2012/06/Rest-api-in-python
    """

    mg_server_url = 'http://localhost:8081/BVH/SetAnimationEntity'
    bvh_file_name = "MGResult.bvh"
    action_file_name = "MGresult_actions.json"
    mg_input_dict = _create_mg_input(bvh_file_name, action_file_name)
    mg_input_string = json.dumps(mg_input_dict)
    request = urllib2.Request(mg_server_url, mg_input_string)
    print "send message and wait for answer..."
    handler = urllib2.urlopen(request)
    result = handler.read()
    print result
    
if __name__ == "__main__":
    send_mg_input()
  