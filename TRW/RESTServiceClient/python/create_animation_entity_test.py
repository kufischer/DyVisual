# -*- coding: utf-8 -*-
"""
Created on Fri Jul 10 15:20:26 2015

@author: erhe01
"""

import urllib2, json

def create_animation_entity():
    """ Based on the example from http://isbullsh.it/2012/06/Rest-api-in-python
    """

    service_url = 'http://localhost:8081/BVH/CreateAnimationEntity'
    input_dict = {
	  "targetAvatarURI":"resources/models/male/male.xml",
      "avatarID":"0"
    }
    input_string = json.dumps(input_dict)
    request = urllib2.Request(service_url, input_string)
    print "send message and wait for answer..."
    handler = urllib2.urlopen(request)
    result = handler.read()
    print result
    
if __name__ == "__main__":
    create_animation_entity()
  