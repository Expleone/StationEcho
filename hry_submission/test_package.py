import json
import os
from typing import List, Dict

PATH_TO_PACKAGE = 'package.json'


class Links:
    def __init__(self, instagram: str = '', facebook: str = '', x: str = '', itchio: str = '',
                 discord: str = '', other: str = ''):
        self.instagram = instagram
        self.facebook = facebook
        self.x = x
        self.itchio = itchio
        self.discord = discord
        self.other = other


class SubmissionInfo:
    def __init__(self, title: str = '', team_number: int = 0, source: str = '',
                 authors: List[Dict[str, str]] = List[Dict[str, str]],
                 term: int = 0, summary: str = '', opensource: bool = False, has_web_build: bool = False,
                 links: Links = Links(), genres: List[str] = List[str]):
        self.title = title
        self.team = team_number
        self.source = source
        if authors is None:
            authors = []
        self.authors = authors
        self.term = term
        self.summary = summary
        self.opensource = opensource
        self.has_web_build = has_web_build
        self.links: Links = links
        self.genres = genres

    def check_keys(self):
        example = SubmissionInfo()
        if set(example.__dict__) - set(self.__dict__) != set():
            raise Exception('Missing keys in: ' + PATH_TO_PACKAGE + ' '
                            + str(set(example.__dict__) - set(self.__dict__)))

    @staticmethod
    def from_json_str(json_str: str):
        info = SubmissionInfo()
        info.__dict__ = json.loads(json_str)
        links = Links()
        links.__dict__ = info.links
        info.links = links
        return info


if __name__ == "__main__":
    if not os.path.isfile(PATH_TO_PACKAGE):
        assert "Could not find " + PATH_TO_PACKAGE + "! Please check that you have not deleted or moved the file."

    # Check if there are any missing keys
    try:
        with open(PATH_TO_PACKAGE, 'r', encoding='utf8') as f:
            info = SubmissionInfo.from_json_str(f.read())
            info.check_keys()
    except Exception as e:
        print('Error while opening file ' + PATH_TO_PACKAGE + ' please check content of the file for any errors!')
        raise e

    # Check if src exists
    if not os.path.isdir(os.path.join('../' + info.source)):
        raise Exception(info.source + ' is not a valid directory!')
