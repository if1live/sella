#-*- coding: utf-8 -*-

u'''
https://wiki.python.org/moin/UdpCommunication
'''

from struct import *
import sys
import socket
import SocketServer

PORT = 15225

u'''
how to run
server : python common.py s
client : python common.py c 127.0.0.1
'''

u'''
서버로 요청 패킷
4 : protocol id
'''

u'''
센서 값 패킷규격(byte : desc)
4 : protocol id,
4 : sequence
4 : yaw
4 : pitch
4 : roll
'''

PACKET_FORMAT_REQUEST = 'i'
PACKET_FORMAT_SENSOR = 'iifff'

PROTOCOL_REQUEST = 0x01
PROTOCOL_SENSOR = 0x02

def show_help(msg=None):
	if msg:
		print msg
	print '%s <c or s> <ip>' % sys.argv[0]

def is_valid_ipv4_address(address):
    try:
        socket.inet_pton(socket.AF_INET, address)
    except AttributeError:  # no inet_pton here, sorry
        try:
            socket.inet_aton(address)
        except socket.error:
            return False
        return address.count('.') == 3
    except socket.error:  # not a valid address
        return False

    return True

def validate_server_argv():
	return True

def validate_client_argv():
	ip = sys.argv[2]
	if not is_valid_ipv4_address(ip):
		show_help('Not valid IP')
		return False
	return True


def main():
	if len(sys.argv) not in (2, 3):
		show_help()
		return -1

	cmd = sys.argv[1]
	if cmd not in ('s', 'c'):
		show_help('Select server or client')
		return -1

	if cmd == 'c':
		valid = validate_client_argv()
		if not valid:
			return -1
		ip = sys.argv[2]
		client_main(ip)
	elif cmd == 's':
		valid = validate_server_argv()
		if not valid:
			return -1
		server_main()

class MyUDPHandler(SocketServer.BaseRequestHandler):
	"""
	This class works similar to the TCP handler class, except that
	self.request consists of a pair of data and client socket, and since
	there is no connection the client address must be given explicitly
	when sending data back via sendto().
	"""
		
	sequence = 1
	yaw = 0.1
	pitch = 0.2
	roll = 0.3
	
	def create_packet(self):
		cls = MyUDPHandler
		data = pack(PACKET_FORMAT_SENSOR, 
			PROTOCOL_SENSOR, 
			cls.sequence, 
			cls.yaw, 
			cls.pitch, 
			cls.roll)
		cls.sequence += 1
		return data

	def handle(self):
		data = self.request[0]
		socket = self.request[1]

		print "{} wrote".format(self.client_address[0])
		data = self.create_packet()
		socket.sendto(data, self.client_address)

def server_main():
	HOST = "0.0.0.0"
	server = SocketServer.UDPServer((HOST, PORT), MyUDPHandler)
	server.serve_forever()

def client_main(ip):
	'''
	sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sock.sendto('test msg', (ip, PORT))
	'''
	HOST = ip
	data = " ".join(sys.argv[1:])

	# SOCK_DGRAM is the socket type to use for UDP sockets
	sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sock.settimeout(1);

	# As you can see, there is no connect() call; UDP has no connections.
	# Instead, data is directly sent to the recipient via sendto().
	data = pack(PACKET_FORMAT_REQUEST, PROTOCOL_REQUEST)
	sock.sendto(data, (HOST, PORT))
	
	received = sock.recv(1024)
	protocol_id, sequence, yaw, pitch, roll = unpack(PACKET_FORMAT_SENSOR, received)

	print 'Sequence : ', sequence
	print 'Yaw = ', yaw
	print 'Pitch = ', pitch
	print 'Roll = ', roll

if __name__ == '__main__':
	main()