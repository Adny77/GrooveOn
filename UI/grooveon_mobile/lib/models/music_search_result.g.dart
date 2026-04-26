// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'music_search_result.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MusicSearchResult _$MusicSearchResultFromJson(Map<String, dynamic> json) =>
    MusicSearchResult(
      items:
          (json['items'] as List<dynamic>?)
              ?.map(
                (e) =>
                    MusicSearchItemResponse.fromJson(e as Map<String, dynamic>),
              )
              .toList() ??
          const [],
      totalCount: (json['totalCount'] as num?)?.toInt(),
    );

Map<String, dynamic> _$MusicSearchResultToJson(MusicSearchResult instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'totalCount': instance.totalCount,
    };
