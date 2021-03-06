# -*- coding: utf-8 -*-
from __future__ import unicode_literals

import web
import sqlite3

from Utils import DBUtils, FileUtils
from Models import Comment

class DownloadComment:
    def GET(self):
        # beg_date = "1997-05-23 03:14:15"
        # end_date = "1997-05-23 03:14:15"
        # channel_id = 'daikun'
        # conn = DBUtils.get_connection()
        # comment_list = Comment.Comment.query_by_period_tuples(conn, beg_date, end_date, channel_id)
        # DBUtils.release_connection(conn)
        return None

        return FileUtils.generate_comment_xml(comment_list)

    def POST(self):
        post = web.input()
        beg_date = post.get('beg_date')
        end_date = post.get('end_date')
        channel_id = post.get('channel_id')

        for key, val in post.items():
            print(key, val)

        conn = DBUtils.get_connection()
        comment_list = Comment.Comment.query_by_period_tuples(conn, beg_date, end_date, channel_id)
        DBUtils.release_connection(conn)

        return FileUtils.generate_comment_xml(comment_list)
